using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebDoDienTu.Areas.Identity.Pages.Account;
using WebDoDienTu.Models;

namespace WebDoDienTu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Create()
        {          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Age = model.Age
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Optionally assign roles or other actions
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public IActionResult Details(string id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }

        public IActionResult Delete(string id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is null or empty");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Age = user.Age,
                Address = user.Address,
                ConcurrencyStamp = user.ConcurrencyStamp
                // Add other properties as needed
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Kiểm tra concurrency stamp
            if (user.ConcurrencyStamp != model.ConcurrencyStamp)
            {
                // Lấy lại thông tin mới nhất của người dùng từ cơ sở dữ liệu
                user = await _userManager.FindByIdAsync(model.Id);

                // Ghi đè các thuộc tính của user
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Age = model.Age;
                user.Address = model.Address;
                user.ConcurrencyStamp = Guid.NewGuid().ToString(); // Cập nhật concurrency stamp để tránh xung đột tiếp theo
            }
            else
            {
                // Cập nhật các thuộc tính của user
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Age = model.Age;
                user.Address = model.Address;
            }

            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes. The user was updated or deleted by another user.");
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> BlockUser(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsBlocked = true;
                _context.Users.Update(user);
                _context.SaveChanges();

                var userPrincipal = await _signInManager.UserManager.GetUserAsync(User);
                if (userPrincipal != null && userPrincipal.Id == userId)
                {
                    await _signInManager.SignOutAsync();
                }

                return RedirectToAction("Index", "Users");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnBlockUser(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsBlocked = false; 
                _context.Users.Update(user); 
                _context.SaveChanges();

                return RedirectToAction("Index", "Users");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
