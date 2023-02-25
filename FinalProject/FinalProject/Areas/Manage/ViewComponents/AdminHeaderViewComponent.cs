using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Localization;

namespace FinalProject.Areas.Manage.ViewComponents
{
    public class AdminHeaderViewComponent : ViewComponent
    {
        private readonly Database _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<AppUser> _userManager;
        public AdminHeaderViewComponent(Database context, IStringLocalizer<SharedResource> localizer, UserManager<AppUser> userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated && (User.IsInRole("SuperAdmin") || User.IsInRole("Admin")))
            {
                user= await _userManager.FindByEmailAsync(User.Identity.Name);
            }
            else return View("Error");
            return View(await Task.FromResult(user));
        }
    }
}
