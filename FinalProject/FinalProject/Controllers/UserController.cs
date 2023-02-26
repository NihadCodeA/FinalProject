using FinalProject.DAL;
using FinalProject.Models;
using FinalProject.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace FinalProject.Controllers
{
    public class UserController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(Database context,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
            _signInManager= signInManager;
            _localizer = localizer;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Update(string Id)
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            AccountDetailsViewModel accountVM = new AccountDetailsViewModel
            {
                Fullname= user.UserName,
                PhoneNumber= user.PhoneNumber,
            };
            return View(accountVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(AccountDetailsViewModel model)
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            if(!ModelState.IsValid) return View(model);
            if (model.Fullname == null)
            {
                ModelState.AddModelError("Fullname","This field is required!");
                return View(model);
            }
            user.FullName=model.Fullname;
            user.PhoneNumber=model.PhoneNumber;
            _context.SaveChanges();
            return RedirectToAction("update","user", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string Id)
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null)
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
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
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
            var changePasswordResult = await _userManager.ChangePasswordAsync(user,passwordVM.OldPassword, passwordVM.NewPassword);
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
            return RedirectToAction("changepassword", "user", new { id = user.Id });
        }
    }
}
