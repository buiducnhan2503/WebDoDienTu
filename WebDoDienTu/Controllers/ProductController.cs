using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index(string keywords)
        {
            // Lấy dữ liệu từ database và gán cho MyModel
            MyModel model = new MyModel();

            if (keywords != null) 
            {
                model.Products = _context.Products.Where(x => x.ProductName.Contains(keywords)).ToList();
                model.Categories = _context.Categories.ToList();
                return View(model);
            }

            // Gán dữ liệu vào MyModel
            model.Products = _context.Products.ToList();
            model.Categories = _context.Categories.ToList();

            return View(model);
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
