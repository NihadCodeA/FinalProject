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
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Member")))
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
        
    }
}
