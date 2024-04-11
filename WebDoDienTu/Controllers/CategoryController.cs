using Microsoft.AspNetCore.Mvc;
using WebDoDienTu.Models;

namespace WebDoDienTu.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductCategory(int id)
        {
            // Lấy dữ liệu từ database và gán cho MyModel
            MyModel model = new MyModel();

            // Gán dữ liệu vào MyModel
            model.Products = _context.Products.Where(x => x.CategoryId == id).ToList();
            model.Categories = _context.Categories.ToList();

            return View(model);
        }
    }
}
