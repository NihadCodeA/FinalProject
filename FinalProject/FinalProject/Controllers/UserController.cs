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

        public UserController(Database context,UserManager<AppUser> userManager, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
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
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Member")))
                {
                    return NotFound();
                }
            }
            else return NotFound();
            AccountDetailsViewModel accountVM = new AccountDetailsViewModel
            {
                User = user,
                Fullname= user.UserName,
                PhoneNumber= user.PhoneNumber,
            };
            return View(accountVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(AccountDetailsViewModel model)
        {
            ViewData["Localizer"] = _localizer;
            if(!ModelState.IsValid) return View(model);
            AppUser user = await _userManager.FindByIdAsync(model.User.Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Member")))
                {
                    return NotFound();
                }
            }
            else return NotFound();
            if (model.Fullname == null)
            {
                ModelState.AddModelError("Fullname","This field is required!");
                return View();
            }
            user.FullName=model.Fullname;
            user.PhoneNumber=model.PhoneNumber;
            _context.SaveChanges();
            return RedirectToAction("update","user", new { id = user.Id });
        }
    }
}
