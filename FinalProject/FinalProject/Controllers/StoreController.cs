using FinalProject.DAL;
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
            List<Product> products = _context.Products.Include(pi=>pi.ProductImages).Where(p=>p.StoreId== storeId).ToList();
            if (store == null) return NotFound();
            StoreViewModel storeViewModel = new StoreViewModel
            {
                Store = store,
                Products= products
            };
            return View(storeViewModel);
        }
        public IActionResult StoreList()
        {
            List<Store> stores = _context.Stores.ToList();
            return View(stores);
        }
    }
}
