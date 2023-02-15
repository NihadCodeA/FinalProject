using FinalProject.Controllers;
using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace FinalProject.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly Database _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public HeaderViewComponent(Database context, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _localizer= localizer;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            ViewData["Dashboard"] = _localizer["Navbar_Dashboard"];
            //----------------------------------------
            string cookieName = ".AspNetCore.Culture";
            if (Request.Cookies.TryGetValue(cookieName, out string value))
            {
                if (value.Contains("az"))
                {
                ViewData["Culture"]= "az";
                }
                else
                {
                ViewData["Culture"]= "en";
                }
            }
            else
            {
                ViewData["Culture"]="az";
            }
            //---------------------------------------- 
            return View(await Task.FromResult(store));
        }
    }
}
