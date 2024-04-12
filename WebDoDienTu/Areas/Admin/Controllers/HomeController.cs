using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebDoDienTu.Models;

namespace WebDoDienTu.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
