using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class StoreController : Controller
    {
        public readonly Database _context;
        public StoreController(Database context)
        {
            _context = context;
        }
        public IActionResult Index(int storeId)
        {
            Store store = _context.Stores.FirstOrDefault(s=> s.Id == storeId);
            if (store == null) return NotFound();
            return View(store);
        }
        public IActionResult StoreList()
        {
            List<Store> stores = _context.Stores.ToList();
            return View(stores);
        }
    }
}
