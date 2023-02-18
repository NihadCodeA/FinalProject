using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (User.Identity.IsAuthenticated)
            {
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Member")))
                {
                    return NotFound();
                }
            }
            else return NotFound();
            return View(user);
        }
    }
}
