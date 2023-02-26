using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.HeaderViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using FinalProject.ViewModels.ProductViewModels;

namespace FinalProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public readonly Database _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly HttpContext _httpContext;
        public ProductController(UserManager<AppUser> userManager, Database context,
            IStringLocalizer<SharedResource> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
            _httpContext = httpContextAccessor.HttpContext;
        }



        public async Task<IActionResult> Detail(int productId)
        {
            if(productId==null) return NotFound();
            Product product = _context.Products.Include(s=>s.Store)
                .Include(pi=>pi.ProductImages).Include(c=>c.Category).FirstOrDefault(p=>p.Id == productId);
            if (product == null) return NotFound();
            Store store = _context.Stores.FirstOrDefault(s=>s.Id==product.StoreId);
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            HeaderViewModel headerVM = new HeaderViewModel
            {
                Store = store,
                Product = product,
                User=appUser,
                Language = GetCurrentLanguage.CurrentLanguage(_httpContext),
                Localizer = _localizer,
                RelatedProdcuts= _context.Products.Include(c=>c.Category).Where(p=>p.CategoryId==product.CategoryId).Take(5).ToList(),
                Categories8 = _context.Categories.Take(8).ToList(),
                Categories16 = _context.Categories.Skip(8).Take(8).ToList(),
                Categories24 = _context.Categories.Skip(16).Take(8).ToList(),
                Categories32 = _context.Categories.Skip(24).Take(8).ToList(),
            };
            return View(headerVM);
        }

        public async Task<IActionResult> AddToBasket(int productId)
        {
            if (!_context.Products.Any(x => x.Id == productId)) return NotFound(); //404
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
            Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == productId);
            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (member == null)
            {
                if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
                basketItem = basketItems.FirstOrDefault(x => x.ProductId == productId);
                if (basketItem != null) basketItem.Count++;
                else
                {
                    basketItem = new BasketItemViewModel
                    {
                        ProductId = productId,
                        Count = 1,
                        Price = product.SalePrice,
                        Discount = product.DiscountPercentage,
                        Name = product.Name,
                        Weight=product.Weight,
                        Image = product.ProductImages.FirstOrDefault(x => x.IsPoster == true)?.Image
                    };
                    basketItems.Add(basketItem);
                }
            }
            else
            {
                basketItem = new BasketItemViewModel
                {
                    ProductId = productId,
                    Count = 1,
                    Price = product.SalePrice,
                    Discount = product.DiscountPercentage,
                    Name = product.Name,
                    Weight = product.Weight,
                    Image = product.ProductImages.FirstOrDefault(x => x.IsPoster == true)?.Image
                };
                basketItems.Add(basketItem);
            }
            basketItemsStr = JsonConvert.SerializeObject(basketItems);
            HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);
            }
            else
            {
                //BasketItem memberBasketItem = _context.BasketItems.Include(x => x.Product).FirstOrDefault(x => x.AppUserId == member.Id && x.Id == productId);
                //if (memberBasketItem != null) memberBasketItem.Count++;
                //else
                //{
                //    memberBasketItem = new BasketItem
                //    {
                //        Id = productId,
                //        AppUserId = member.Id,
                //        Count = 1,
                //        Price = product.SalePrice,
                //        Discount = product.DiscountPercentage,
                //        Name = product.Name,
                //        Weight = product.Weight,
                //        Image = product.ProductImages.FirstOrDefault(x => x.IsPoster == true)?.Image
                //    };
                //    _context.BasketItems.Add(memberBasketItem);
                //}
                BasketItem memberBasketItem = _context.BasketItems.Include(x => x.Product)
                     .FirstOrDefault(x => x.AppUserId == member.Id && x.Product.Id == productId);

                if (memberBasketItem != null)
                {
                    memberBasketItem.Count++;
                }
                else
                {
                    memberBasketItem = new BasketItem
                    {
                        AppUserId = member.Id,
                        ProductId = productId,
                        Count = 1,
                        Price = product.SalePrice,
                        Discount = product.DiscountPercentage,
                        Name = product.Name,
                        Weight = product.Weight,
                        Image = product.ProductImages.FirstOrDefault(x => x.IsPoster == true)?.Image
                    };
                    _context.BasketItems.Add(memberBasketItem);
                }
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}
