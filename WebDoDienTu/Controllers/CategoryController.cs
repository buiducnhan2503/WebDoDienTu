using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
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

        public IActionResult ProductCategory(string category)
        {
            var item = _context.Products
            .Include(p => p.Category)
            .AsEnumerable()
                    .Where(x => x.Category.CategoryName.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            return View(item);
        }
    }
}
