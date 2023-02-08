using FinalProject.DAL;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        public readonly Database _context ;
        public HomeController(Database context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}