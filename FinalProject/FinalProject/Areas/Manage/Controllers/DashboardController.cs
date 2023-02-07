using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DashboardController(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateAdmin()
        {
            AppUser admin = new AppUser
            {
                FullName = "Nihad Balakişiyev",
                UserName = "SuperAdmin",
                Email = "nihadbalakisiyev18@gmail.com"
            };
            var result = await _userManager.CreateAsync(admin, password: "Nihad2003");
            return Ok(result);
        }
        public async Task<IActionResult> CreateRoles()
        {
            IdentityRole member = new IdentityRole("Member");
            IdentityRole store = new IdentityRole("Store");
            IdentityRole admin = new IdentityRole("Admin");
            IdentityRole superAdmin = new IdentityRole("SuperAdmin");
            await _roleManager.CreateAsync(member);
            await _roleManager.CreateAsync(store);
            await _roleManager.CreateAsync(admin);
            var result = await _roleManager.CreateAsync(superAdmin);
            return Ok(result);
        }

        public async Task<IActionResult> AddRole()
        {
            var user = await _userManager.FindByNameAsync("SuperAdmin");

            var result = await _userManager.AddToRoleAsync(user, "SuperAdmin");

            return Ok(result);
        }
    }
}
