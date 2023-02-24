using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.StoreViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;

namespace FinalProject.Areas.StoreManage.Controllers
{
    [Area("StoreManage")]
    [Authorize(Roles ="Store")]
    public class HomeController : Controller
    {
        private readonly Database _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        public HomeController(Database context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }
        public IActionResult Index(int storeId)
        {
            ViewData["PageName"] = "Dashboard";
            if (_context.Stores.FirstOrDefault(s => s.Id == storeId) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == storeId);
            if (store == null) return NotFound();
            return View(store);
        }


    }
}
