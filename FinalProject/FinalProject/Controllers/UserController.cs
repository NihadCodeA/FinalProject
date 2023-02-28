using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using static System.Formats.Asn1.AsnWriter;

namespace FinalProject.Controllers
{
    public class UserController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly HttpContext _httpContext;

        public UserController(Database context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
            _userManager = userManager;
            _signInManager= signInManager;
            _localizer = localizer;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Update(string Id)
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            AccountDetailsViewModel accountVM = new AccountDetailsViewModel
            {
                Fullname= user.UserName,
                PhoneNumber= user.PhoneNumber,
            };
            return View(accountVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(AccountDetailsViewModel model)
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            if(!ModelState.IsValid) return View(model);
            if (model.Fullname == null)
            {
                ModelState.AddModelError("Fullname","This field is required!");
                return View(model);
            }
            user.FullName=model.Fullname;
            user.PhoneNumber=model.PhoneNumber;
            _context.SaveChanges();
            return RedirectToAction("update","user", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string Id)
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(NewPasswordViewModel passwordVM) 
        {
            ViewData["Localizer"] = _localizer;
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            if (!ModelState.IsValid)
            {

                TempData["ChangePasswordResult"] = "false";
                return View();
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user,passwordVM.OldPassword, passwordVM.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    TempData["ChangePasswordResult"] = "false";
                    return View();
                }
            }
            else TempData["ChangePasswordResult"] = "true";

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("changepassword", "user", new { id = user.Id });
        }

        public async Task<IActionResult> Orders(string Id,int page = 1)
        {
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["Localizer"] = _localizer;
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;
            var query = _context.Orders.Include(o => o.AppUser).Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Store).Where(o => o.AppUserId == user.Id).AsQueryable();
            var pagenatedOrders = PaginatedList<Order>.Create(query, 5, page);

            return View(pagenatedOrders);
        }
        public async Task<IActionResult> OrderDetail(string Id, int orderId)
        {
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["Localizer"] = _localizer;
            AppUser user = await _userManager.FindByIdAsync(Id);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null)
                {
                    return NotFound();
                }
            }
            else return NotFound();
            ViewData["User"] = user;

            //List<OrderItem> userOrders = _context.OrderItems.Include(o => o.Order).Include(p => p.Product)
            //   .ThenInclude(pi => pi.ProductImages).Where(s => s.StoreId == store.Id && s.OrderId == orderId && s.Count > 0).ToList();

            //Order order = _context.Orders
            //.Include(o => o.AppUser).Where(x=>x.AppUserId== user.Id).Include(x=>x.OrderItems).ThenInclude(x=>x.Product)
            //.ThenInclude(pi=>pi.ProductImages).FirstOrDefault(o => o.Id == orderId);

            Order order = _context.Orders
            .Include(o => o.AppUser)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Store)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
            .FirstOrDefault(o => o.Id == orderId);
            ViewData["Order"] = order;
            return View();
        }

    }
}
