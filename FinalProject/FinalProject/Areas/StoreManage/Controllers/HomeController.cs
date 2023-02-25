using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.StoreViewModels;
using FinalProject.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using QRCoder;
using System.Drawing;

namespace FinalProject.Areas.StoreManage.Controllers
{
    [Area("StoreManage")]
    [Authorize(Roles ="Store")]
    public class HomeController : Controller
    {
        private readonly Database _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(Database context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env,
            UserManager<AppUser> userManager,IStringLocalizer<SharedResource> localizer, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _userManager = userManager;
            _signInManager=signInManager;
            _localizer=localizer;
        }
        public IActionResult Index(int storeId)
        {
            ViewData["PageName"] = "Dashboard";
            ViewData["Localizer"] = _localizer;
            if (_context.Stores.FirstOrDefault(s => s.Id == storeId) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == storeId);
            if (store == null) return NotFound();
            if (!(User.Identity.IsAuthenticated && User.IsInRole("Store") && store.Email == User.Identity.Name))
            {
                return NotFound();
            }
            return View(store);
        }

        public IActionResult UpdateInfo(int id)
        {
            ViewData["PageName"] = "Settings";
            ViewData["Localizer"] = _localizer;
            if (id == null) return NotFound();
            if (_context.Stores.FirstOrDefault(s => s.Id == id) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == id);
            if (!(User.Identity.IsAuthenticated && User.IsInRole("Store") && store.Email == User.Identity.Name))
            {
                return NotFound();
            }
            return View(store);
        }
        [HttpPost]
        public IActionResult UpdateInfo(Store store)
        {
            ViewData["PageName"] = "Settings";
            if (!ModelState.IsValid) return View();

            Store existStore = _context.Stores.FirstOrDefault(x => x.Id == store.Id);
            if (existStore == null) return NotFound();

            //---------------------------------------------------------
            if (store.LogoImageId == null && existStore.LogoImage != null)
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/stores", existStore.LogoImage);
                existStore.LogoImage = null;
            }
            //-------------------------------------------------------
            if (store.LogoImageFile != null)
            {
                if (store.LogoImageFile.ContentType != "image/png" && store.LogoImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFiles", "Ancaq png ve ya jpeg (jpg) formatinda olan sekilleri yukleye bilersiniz!");
                    return View();
                }
                if (store.LogoImageFile.Length > 3145728)
                {
                    ModelState.AddModelError("ImageFiles", "Seklin olcusu 3mb-den cox ola bilmez!");
                    return View();
                }
                if (existStore.LogoImage != null)
                {
                    FileManager.DeleteFile(_env.WebRootPath, "uploads/stores", existStore.LogoImage);
                    existStore.LogoImage = FileManager.SaveFile(_env.WebRootPath, "uploads/stores", store.LogoImageFile);
                }
                else
                {
                    existStore.LogoImage = FileManager.SaveFile(_env.WebRootPath, "uploads/stores", store.LogoImageFile);
                }
            }

            if (store.StoreName == null)
            {
                ModelState.AddModelError("StoreName", "StoreName field is required!");
                return View();
            }
            if (store.Address == null)
            {
                ModelState.AddModelError("Address", "Address field is required!");
                return View();
            }
            existStore.StoreName = store.StoreName;
            existStore.PhoneNumber1 = store.PhoneNumber1;
            existStore.PhoneNumber2 = store.PhoneNumber2;
            existStore.PhoneNumber3 = store.PhoneNumber3;
            existStore.PhoneNumber4 = store.PhoneNumber4;
            existStore.Address = store.Address;
            existStore.Description = store.Description;
            _context.SaveChanges();
            return RedirectToAction("updateinfo", "home", new {area="storemanage", id = existStore.Id });

        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string Id)
        {
            ViewData["Localizer"] = _localizer;
            ViewData["PageName"] = "ChangePassword";
            ViewData["Localizer"] = _localizer;
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Store")))
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(NewPasswordViewModel passwordVM)
        {
            ViewData["Localizer"] = _localizer;
            ViewData["PageName"] = "ChangePassword";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Store")))
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            if (!ModelState.IsValid)
            {

                TempData["ChangePasswordResult"] = "false";
                return View();
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, passwordVM.OldPassword, passwordVM.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    TempData["ChangePasswordResult"] = "false";
                    return View();
                }
            }
            else TempData["ChangePasswordResult"] = "true";

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("changepassword", "home", new {area="storemanage", id = user.Id });
        }
    }
}
