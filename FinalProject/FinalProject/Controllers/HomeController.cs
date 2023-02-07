using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        public readonly Database _context ;
        public readonly UserManager<AppUser> _userManager;
        public HomeController(Database context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StoreList()
        {
            //List<AppUser> stores = _context.Users.Where(r => r.IsStore == true).ToList();
            List<AppUser> stores = _userManager.GetUsersInRoleAsync("Store").Result.ToList();
            return View(stores);
        }
    }
}