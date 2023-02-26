using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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
            List<Slider> sliders= _context.Sliders.OrderBy(x=>x.Order).ToList();
            HomeViewModel homeVM = new HomeViewModel {
                Sliders = sliders,
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
    }
}