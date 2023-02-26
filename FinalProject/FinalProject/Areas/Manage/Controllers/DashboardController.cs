using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace FinalProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    public class DashboardController : Controller
    {
        private readonly Database _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public DashboardController(Database context,UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _context=context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["PageName"] = "Dashboard";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            else return NotFound();
            ViewData["User"]=user;
            return View(user);
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Stores(int page=1,string? storeName=null)
        {
            ViewData["PageName"] = "Stores";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            var query = _context.Stores.AsQueryable();
            if(storeName!= null)
            {
                query=_context.Stores.Where(s=>s.StoreName.Contains(storeName)).AsQueryable();
            }
            var pagenatedStores = PaginatedList<Store>.Create(query, 15, page);
            return View(pagenatedStores);
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Users(int page=1,string? userName=null)
        {
            ViewData["PageName"] = "Users";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            var query = _userManager.GetUsersInRoleAsync("Member").Result.AsQueryable();
            if (userName!= null)
            {
                query = _userManager.GetUsersInRoleAsync("Member").Result.Where(u=>u.FullName.Contains(userName)).AsQueryable();
            }
            var pagenatedUsers = PaginatedList<AppUser>.Create(query, 15, page);
            return View(pagenatedUsers);
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Admins(int page=1,string? adminName=null)
        {
            ViewData["PageName"] = "Admins";
            AppUser user = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (user == null) return NotFound();
            ViewData["User"] = user;
            var query = _userManager.GetUsersInRoleAsync("Admin").Result.AsQueryable();
            if (adminName!= null)
            {
                query = _userManager.GetUsersInRoleAsync("Admin").Result.Where(u=>u.FullName.Contains(adminName)).AsQueryable();
            }
            var pagenatedAdmins = PaginatedList<AppUser>.Create(query, 15, page);
            return View(pagenatedAdmins);
        }


        [HttpGet]
        public async Task<IActionResult> Update(string Id)
        {
            ViewData["PageName"] = "Settings";
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
                Fullname = user.UserName,
                PhoneNumber = user.PhoneNumber,
            };
            return View(accountVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(AccountDetailsViewModel model)
        {
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
            if (!ModelState.IsValid) return View(model);
            if (model.Fullname == null)
            {
                ModelState.AddModelError("Fullname", "yazılmalıdır");
                return View(model);
            }
            user.FullName = model.Fullname;
            user.PhoneNumber = model.PhoneNumber;
            _context.SaveChanges();
            return RedirectToAction("update", "dashboard", new {area="manage", id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string Id)
        {
            ViewData["PageName"] = "ChangePassword";
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
            ViewData["PageName"] = "ChangePassword";
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
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, passwordVM.OldPassword, passwordVM.NewPassword);
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
            return RedirectToAction("changepassword", "dashboard", new { area = "manage", id = user.Id });
        }


        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser admin = new AppUser
        //    {
        //        FullName = "Nihad Balakişiyev",
        //        UserName = "SuperAdmin",
        //        Email = "nihadbalakisiyev18@gmail.com"
        //    };
        //    var result = await _userManager.CreateAsync(admin, password: "Nihad2003");
        //    return Ok(result);
        //}
        //public async Task<IActionResult> CreateRoles()
        //{
        //    IdentityRole member = new IdentityRole("Member");
        //    IdentityRole store = new IdentityRole("Store");
        //    IdentityRole admin = new IdentityRole("Admin");
        //    IdentityRole superAdmin = new IdentityRole("SuperAdmin");
        //    await _roleManager.CreateAsync(member);
        //    await _roleManager.CreateAsync(store);
        //    await _roleManager.CreateAsync(admin);
        //    var result = await _roleManager.CreateAsync(superAdmin);
        //    return Ok(result);
        //}

        //public async Task<IActionResult> AddRole()
        //{
        //    var user = await _userManager.FindByNameAsync("SuperAdmin");

        //    var result = await _userManager.AddToRoleAsync(user, "SuperAdmin");

        //    return Ok(result);
        //}
    }
}
