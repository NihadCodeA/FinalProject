using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.HeaderViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Localization;
namespace FinalProject.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Database _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly HttpContext _httpContext;
        public HeaderViewComponent(UserManager<AppUser> userManager, Database context,
            IStringLocalizer<SharedResource> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
            _httpContext = httpContextAccessor.HttpContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            HeaderViewModel headerVM = new HeaderViewModel
            {
                Store = store,
                User = appUser,
                Language = GetCurrentLanguage.CurrentLanguage(_httpContext),
                Localizer = _localizer,
            };
            return View(await Task.FromResult(headerVM));
        }
    }
}