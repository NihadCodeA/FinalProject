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
        private readonly IWebHostEnvironment _env;

        public AdminController(Database context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
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


        public async Task<IActionResult> Categories(int page=1,string? categoryName=null)
        {
            //---------------------------------
            ViewData["PageName"] = "Categories";
            AppUser superadmin = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                superadmin = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (superadmin == null) return NotFound();
            ViewData["User"] = superadmin;
            //-------------------------------
            var query = _context.Categories.AsQueryable();
            if (categoryName != null)
            {
                query = _context.Categories.Where(c => c.NameAz.Contains(categoryName)).AsQueryable();
            }
            var pagenatedCategories = PaginatedList<Category>.Create(query, 5, page);
            return View(pagenatedCategories);
        }
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            //---------------------------------
            ViewData["PageName"] = "Categories";
            AppUser superadmin = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                superadmin = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (superadmin == null) return NotFound();
            ViewData["User"] = superadmin;
            //-------------------------------
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            //---------------------------------
            ViewData["PageName"] = "Categories";
            AppUser superadmin = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                superadmin = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (superadmin == null) return NotFound();
            ViewData["User"] = superadmin;
            //-------------------------------
            if (!ModelState.IsValid) return View();
            if (category.ImageFile != null)
            {
                if (category.ImageFile.Length > 3145728)
                {
                    ModelState.AddModelError("ImageFile", "Şəklin ölçüsü 3mb-dən çox ola bilməz!");
                    return View();
                }
                category.Image = FileManager.SaveFile(_env.WebRootPath, "uploads/categories", category.ImageFile);
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Şəkil yükləmək məcburidi!");
                return View();
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Categories") ;
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //------------------------------
            ViewData["PageName"] = "Categories";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------
            Category category = _context.Categories.Find(id);
            if (category == null) View("Error");
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            //------------------------------
            ViewData["PageName"] = "Categories";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------
            Category existCategory= _context.Categories.Find(category.Id);
            if (existCategory == null) View("Error");
            if (!ModelState.IsValid) return View();
            if (category.ImageFile != null)
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/categories", existCategory.Image);
                if (category.ImageFile.Length > 3145728)
                {
                    ModelState.AddModelError("PosterImgFile", "Şəklin ölçüsü 3mb-dən çox ola bilməz!");
                    return View();
                }
                existCategory.Image = FileManager.SaveFile(_env.WebRootPath, "uploads/categories", category.ImageFile);
            }
            existCategory.NameAz = category.NameAz;
            existCategory.NameEn = category.NameEn;
            _context.SaveChanges();
            return RedirectToAction("Categories");
        }

        public async Task<IActionResult> Delete(int id)
        {
            //------------------------------
            ViewData["PageName"] = "Categories";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            //------------------------------

            Category category = _context.Categories.Find(id);
            if (category == null) NotFound(); //404
            if (category.Image != null)
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/categories", category.Image);
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Categories");
        }
    }
}
