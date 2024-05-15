using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;
using WebDoDienTu.Models;
using WebDoDienTu.Models.Repository;

namespace WebDoDienTu.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị tất cả sản phẩm sản phẩm
        public IActionResult Index(string? keywords, string? priceRange, int? page)
        {
            int pageSize = 10; // Số lượng sản phẩm trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là 1 nếu không có

            if (keywords != null) 
            {
                var item = _context.Products.Where(x => x.ProductName.Contains(keywords)).ToPagedList(pageNumber, pageSize);
                return View(item);
            }

            if(priceRange != null)
            {
                var priceLimits = priceRange.Split('-').Select(int.Parse).ToList();
                var minPrice = priceLimits[0];
                var maxPrice = priceLimits[1];

                // Lọc sản phẩm theo khoảng giá
                var item = _context.Products.Where(x => x.Price >= minPrice && x.Price < maxPrice).ToPagedList(pageNumber, pageSize);
                return View(item);
            }
            var products = _context.Products.ToPagedList(pageNumber, pageSize);
            return View(products);
        }

        // Hiển thị chi tiết một sản phẩm
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
