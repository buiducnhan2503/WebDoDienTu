﻿using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDoDienTu.Extensions;
using WebDoDienTu.Models;
using WebDoDienTu.Models.Repository;
using WebDoDienTu.Service;

namespace WebDoDienTu.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVnPayService _vnPayService;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository, IVnPayService vnPayService)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _vnPayService = vnPayService;
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
            var product = await GetProductFromDatabase(productId);
            var cartItem = new CartItem
            {
                ProductId = productId,
                NameProduct = product.ProductName,
                Image = product.ImageUrl ?? string.Empty,
                Price = product.Price,
                Quantity = quantity
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }
        
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart is not null)
            {
                cart.RemoveItem(productId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Checkout(decimal finalPrice)
        {
            return View(new Order());
        }

        public static Order orderTemp = new Order();

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, decimal finalPrice, string payment = "COD")
        {
            if(ModelState.IsValid)
            {
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

                if (cart == null || !cart.Items.Any())
                {
                    // Xử lý giỏ hàng trống...
                    TempData["EmptyCartMessage"] = "Giỏ hàng của bạn hiện đang trống.";
                    return RedirectToAction("Index");
                }
                
                if (payment == "Thanh toán VNPay")
                {
                    orderTemp = order;
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = (double)cart.Items.Sum(i => i.Price * i.Quantity),
                        CreatedDate = DateTime.Now,
                        Description = $"{order.FirstName} {order.LastName} {order.Phone}",
                        FullName = $"{order.FirstName} {order.LastName}",
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    order.UserId = user.Id;
                    order.OrderDate = DateTime.UtcNow;
                    order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
                    order.OrderDetails = cart.Items.Select(i => new OrderDetail
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        Price = i.Price
                    }).ToList();
                }
                    
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");
                TempData["Message"] = $"Thanh toán thành công";
                return View("OrderCompleted", order.Id);             

            }
            TempData["ModelState"] = "Vui lòng điền đầy đủ thông tin.";
            return View(order);
        }

        public async Task<IActionResult> PaymentCallBack()
        {
            if (ModelState.IsValid)
            {
                var response = _vnPayService.PaymentExecute(Request.Query);
                var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

                if (response == null || response.VnPayResponseCode != "00")
                {
                    TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                    return RedirectToAction("PaymentFail");
                }
                Order ordervnPay = new Order();
                var user = await _userManager.GetUserAsync(User);
                ordervnPay.UserId = user.Id;
                ordervnPay.OrderDate = DateTime.UtcNow;
                ordervnPay.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
                ordervnPay.FirstName = orderTemp.FirstName;
                ordervnPay.LastName = orderTemp.LastName;
                ordervnPay.Phone = orderTemp.Phone;
                ordervnPay.Email = orderTemp.Email;
                ordervnPay.Address = orderTemp.Address;
                ordervnPay.OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList();


                _context.Orders.Add(ordervnPay);
                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");

                TempData["Message"] = $"Thanh toán VNPay thành công";
                return View("OrderCompleted");
            }
            TempData["Message"] = "Lỗi thanh toán VN Pay";
            return RedirectToAction("PaymentFail");
        }

        public IActionResult PaymentFail()
        {
            return View();
        }
    }
}
