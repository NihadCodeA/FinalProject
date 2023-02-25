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
                .Include(pi=>pi.ProductImages).FirstOrDefault(p=>p.Id == productId);
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
            };
            return View(headerVM);
        }
    }
}
