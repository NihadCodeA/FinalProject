using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Localization;

namespace FinalProject.Areas.StoreManage.ViewComponents
{
    public class StoreManageHeaderViewComponent : ViewComponent
    {
        private readonly Database _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly UserManager<AppUser> _userManager;
        public StoreManageHeaderViewComponent(Database context, IStringLocalizer<SharedResource> localizer, UserManager<AppUser> userManager)
        {
            _context = context;
            _localizer = localizer;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["Localizer"] = _localizer;
            Store store = new Store();
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
                user= await _userManager.FindByEmailAsync(User.Identity.Name);
            };
            if (User.Identity.IsAuthenticated)
            {
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Store")))
                {
                    return View("Error");
                }
            }
            else return View("Error");
            ViewData["User"] = user;
            return View(await Task.FromResult(store));
        }
    }
}
