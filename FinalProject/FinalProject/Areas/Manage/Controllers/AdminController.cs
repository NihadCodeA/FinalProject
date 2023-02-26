using FinalProject.Areas.Manage.ViewModels;
using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FinalProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(Database context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            //---------------------------------------
            ViewData["PageName"] = "Admins";
            AppUser superadmin = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                superadmin = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (superadmin == null) return NotFound();
            ViewData["User"] = superadmin;
            //------------------------------------
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateAdmin(AdminRegisterViewModel adminVM)
        {
            //---------------------------------
            ViewData["PageName"] = "Admins";
            AppUser superadmin = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                superadmin = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (superadmin == null) return NotFound();
            ViewData["User"] = superadmin;
            //-------------------------------
            if (!ModelState.IsValid) return View();
            var user = await _userManager.FindByEmailAsync(adminVM.Email);
            if (user != null)
            {
                ModelState.AddModelError("Email", "Eyni email adresini başqa bir hesap istifadə eliyir.");
                return View();
            }
            user = new AppUser
            {
                FullName = adminVM.Fullname,
                Email = adminVM.Email,
                UserName = adminVM.Email,
                EmailConfirmed=true,
            };
            
            var passwordResult = await _userManager.CreateAsync(user, adminVM.Password);
            if (!passwordResult.Succeeded)
            {
                if (adminVM.Password.Length < 8)
                {
                    ModelState.AddModelError("", "Şifrə minimum 8 simvoldan ibarət olmalıdır!");
                }
                if (!adminVM.Password.Any(char.IsUpper))
                {
                    ModelState.AddModelError("", "Şifrədə minimum 1 böyük hərf olmalıdır!");
                }
                if (!adminVM.Password.Any(char.IsLower))
                {
                    ModelState.AddModelError("", "Şifrədə minimum 1 kiçik hərf olmalıdır!");
                }
                if (!adminVM.Password.Any(char.IsDigit))
                {
                    ModelState.AddModelError("", "Şifrədə minimum 1 rəqəm olmalıdır!");
                }
                return View();
            }
            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
            foreach (var err in roleResult.Errors)
            {
                ModelState.AddModelError("", err.Description);
                return View();
            }
            return RedirectToAction("Admins", "Dashboard", new { area = "manage" });
        }
    }
}
