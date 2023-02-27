using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.HeaderViewModels;
using FinalProject.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

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
            //------------------------------------------------------------------------------
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItemViewModel basketItem = null;
            if(User.Identity.IsAuthenticated ==false)
            {
                string basketItemStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                }

            }
            else
            {
                 basketItems = _context.BasketItems.Where(x => x.AppUserId == appUser.Id).ToList();
                foreach(var item in basketItems)
                {
                    basketItem = new BasketItemViewModel
                    {
                        ProductId=(int)item.ProductId ,
                        Count =item.Count,
                        Price = item.Price,
                        Discount = item.Discount ,
                        Name = item.Name ,
                        Weight = item.Weight,
                        Image = item.Image,
                    };
                    basketItemsVM.Add(basketItem);
                }
            }
            //---------------------------------------------------------------------------------
            HeaderViewModel headerVM = new HeaderViewModel
            {
                Store = store,
                User = appUser,
                Language = GetCurrentLanguage.CurrentLanguage(_httpContext),
                Localizer = _localizer,
                Categories8 = _context.Categories.Take(8).ToList(),
                Categories16=_context.Categories.Skip(8).Take(8).ToList(),
                Categories24=_context.Categories.Skip(16).Take(8).ToList(),
                Categories32=_context.Categories.Skip(24).Take(8).ToList(),
                BasketItemViewModels=basketItemsVM,
            };
            return View(await Task.FromResult(headerVM));
        }
    }
}