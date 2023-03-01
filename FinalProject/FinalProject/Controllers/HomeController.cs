using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly Database _context ;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly HttpContext _httpContext;
        public HomeController(Database context, IStringLocalizer<SharedResource> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _localizer = localizer;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public IActionResult Index()
        {
            ViewData["Dashboard"] = _localizer.GetString("Navbar_Dashboard");
            List<Slider> sliders = _context.Sliders.OrderBy(x=>x.Order).ToList();
            List<Category> categories = _context.Categories.ToList();
            List<Product> products = _context.Products.Include(pi=>pi.ProductImages).Include(c=>c.Category).Take(10).ToList();
            List<Product> discountedProducts = _context.Products.Include(pi=>pi.ProductImages).Include(c=>c.Category).Where(t=>t.DiscountEndingDate>DateTime.Now).Take(4).ToList();
            HomeViewModel homeVM = new HomeViewModel {
                Sliders = sliders,
                Categories = categories,
                PopularProducts = products,
                DiscountedProducts = discountedProducts,
                Localizer =_localizer,
                Lang = GetCurrentLanguage.CurrentLanguage(_httpContext),
            };
            return View(homeVM);
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }
            );
            string returnUrl = Request.Headers["Referer"].ToString();
            return Redirect(returnUrl);
        }

        public ActionResult SearchProducts(int categoryId,int page=1,string? productName=null
            ,double? minPrice=null,double? maxPrice=null)
        {
            if (_context.Categories.FirstOrDefault(c => c.Id == categoryId) == null)
            {
                return NotFound();
            }
            var products = _context.Products.AsQueryable();

            if (productName != null) products = products.Where(x => x.Name.ToLower().Contains(productName.ToLower()));
            
            if (minPrice != null) products = products.Where(x => x.SalePrice>=minPrice);
            
            if (maxPrice != null) products = products.Where(x => x.SalePrice<=maxPrice);
            
            var query = products.Include(pi => pi.ProductImages).Include(c => c.Store)
               .Where(p => p.CategoryId == categoryId && p.IsAvaible==true).AsQueryable();

            var pagenatedProducts = PaginatedList<Product>.Create(query, 16, page);
            ViewBag.Categories = _context.Categories.ToList();
            ViewData["CategoryNameAz"] = _context.Categories.FirstOrDefault(c => c.Id == categoryId).NameAz;
            ViewData["CategoryNameEn"] = _context.Categories.FirstOrDefault(c => c.Id == categoryId).NameEn;
            ViewData["CategoryId"] = _context.Categories.FirstOrDefault(c => c.Id == categoryId).Id;
            ViewData["productCount"] = query.Count();
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            return View(pagenatedProducts);
        }
    }
}