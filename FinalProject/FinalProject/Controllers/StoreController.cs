using FinalProject.DAL;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
            if(_context.Stores.FirstOrDefault(s => s.Id == storeId)==null)
            {
                return NotFound();
            }
            //Response.Cookies.Append("StoreId",storeId.ToString());
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
