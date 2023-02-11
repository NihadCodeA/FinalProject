using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.StoreViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class StoreController : Controller
    {
        public readonly Database _context;
        public StoreController(Database context)
        {
            _context = context;
        }
        public IActionResult Index(int storeId,int page=1)
        {
            if(_context.Stores.FirstOrDefault(s => s.Id == storeId)==null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s=> s.Id == storeId);
            var query = _context.Products.Include(pi=>pi.ProductImages).Where(p=>p.StoreId== storeId).AsQueryable();
            var pagenatedProducts = PaginatedList<Product>.Create(query, 12, page);
            ViewData["ProductsCount"] = query.Count();
            if (store == null) return NotFound();
            StoreViewModel storeViewModel = new StoreViewModel
            {
                Store = store,
                Products= pagenatedProducts
            };
            return View(storeViewModel);
        }
        public IActionResult StoreList()
        {
            List<Store> stores = _context.Stores.ToList();
            return View(stores);
        }

        public IActionResult Info(int storeId)
        {
            if (_context.Stores.FirstOrDefault(s => s.Id == storeId) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == storeId);
            return View(store);
        }
        [HttpGet]
        public IActionResult UpdateInfo(int storeId)
        {
            if (_context.Stores.FirstOrDefault(s => s.Id == storeId) == null)
            {
                return NotFound();
            }
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();
            return View(store);
        }
        [HttpPost]
        public IActionResult UpdateInfo(Store store)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();
            return RedirectToAction(nameof(Info));
        }
    }
}
