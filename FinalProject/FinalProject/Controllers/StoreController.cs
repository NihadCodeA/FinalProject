using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.StoreViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class StoreController : Controller
    {
        private readonly Database _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        public StoreController(Database context, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }
        public IActionResult Index(int storeId, int page = 1)
        {
            if (_context.Stores.FirstOrDefault(s => s.Id == storeId) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == storeId);
            var query = _context.Products.Include(pi => pi.ProductImages).Where(p => p.StoreId == storeId).AsQueryable();
            var pagenatedProducts = PaginatedList<Product>.Create(query, 12, page);
            ViewData["ProductsCount"] = query.Count();
            if (store == null) return NotFound();
            StoreViewModel storeViewModel = new StoreViewModel
            {
                Store = store,
                Products = pagenatedProducts,
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
            if (storeId == null) return NotFound();
            if (_context.Stores.FirstOrDefault(s => s.Id == storeId) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == storeId);
            if (!(User.Identity.IsAuthenticated && User.IsInRole("Store") && store.Email == User.Identity.Name))
            {
                return NotFound();
            }
            return View(store);
        }
        [HttpPost]
        public IActionResult UpdateInfo(Store store)
        {
            if (!ModelState.IsValid) return View();

            Store existStore = _context.Stores.FirstOrDefault(x => x.Id == store.Id);
            if (existStore == null) return NotFound();

            //---------------------------------------------------------
            if (store.LogoImageId!= null)
            {
                existStore.LogoImage = null;
                FileManager.DeleteFile(_env.WebRootPath, "uploads/stores",existStore.LogoImage);
            }
            //-------------------------------------------------------
            if (store.LogoImage != null)
            {
                if (store.LogoImageFile.ContentType != "image/png" && store.LogoImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFiles", "Ancaq png ve ya jpeg (jpg) formatinda olan sekilleri yukleye bilersiniz!");
                    return View();
                }
                if (store.LogoImageFile.Length > 3145728)
                {
                    ModelState.AddModelError("ImageFiles", "Seklin olcusu 3mb-den cox ola bilmez!");
                    return View();
                }
                if (existStore.LogoImage != null)
                {
                    FileManager.DeleteFile(_env.WebRootPath, "uploads/stores", existStore.LogoImage);
                    existStore.LogoImage = FileManager.SaveFile(_env.WebRootPath, "uploads/stores", store.LogoImageFile);
                }
                else
                {
                    existStore.LogoImage = FileManager.SaveFile(_env.WebRootPath, "uploads/stores", store.LogoImageFile);
                }
            }

            if (store.StoreName == null)
            {
                ModelState.AddModelError("StoreName","StoreName field is required!");
                return View();
            }
            if (store.Address == null)
            {
                ModelState.AddModelError("Address", "Address field is required!");
                return View();
            }
            existStore.StoreName = store.StoreName;
            existStore.PhoneNumber1 = store.PhoneNumber1;
            existStore.PhoneNumber2 = store.PhoneNumber2;
            existStore.PhoneNumber3 = store.PhoneNumber3;
            existStore.PhoneNumber4 = store.PhoneNumber4;
            existStore.Address = store.Address;
            existStore.Description = store.Description;
            _context.SaveChanges();
            return RedirectToAction("Info");
        }
    }
}
