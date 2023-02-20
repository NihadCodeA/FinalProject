using FinalProject.DAL;
using FinalProject.Models;
using FinalProject.ViewModels.HeaderViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Asn1.Ocsp;

namespace FinalProject.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Database _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public FooterViewComponent(UserManager<AppUser> userManager, Database context, IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            //----------------------------------------
            string cookieName = ".AspNetCore.Culture";
            string language = "az";
            if (Request.Cookies.TryGetValue(cookieName, out string value))
            {
                if (value.Contains("az"))
                {
                    language = "az";
                }
                else
                {
                    language = "en";
                }
            }
            else
            {
                language = "az";
            }
            //---------------------------------------- 
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            return View(await Task.FromResult(appUser));
        }
    }
}