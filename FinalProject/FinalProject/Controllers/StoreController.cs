using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.StoreViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
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

            //===========QR code generate ===============
            string QRtext = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + HttpContext.Request.Path.Value + HttpContext.Request.QueryString.Value;
            QRCodeGenerator QRGen = new QRCodeGenerator();
            QRCodeData Qrinfo = QRGen.CreateQrCode(QRtext, QRCodeGenerator.ECCLevel.Q);
            QRCode qRCoder = new QRCode(Qrinfo);

            Bitmap QRbitmap = qRCoder.GetGraphic(50);

            // Color 
            //Bitmap QRbitmap = qRCoder.GetGraphic(50, Color.Blue, Color.Gray, true);

            byte[] bitmapArray = BitmapToBytes(QRbitmap);
            var Qrcodeimage = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bitmapArray));
            ViewBag.QRCodeImage = Qrcodeimage;
            //===========================================
            StoreViewModel storeViewModel = new StoreViewModel
            {
                Store = store,
                Products = pagenatedProducts,
            };
            return View(storeViewModel);
        }
        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
        public IActionResult StoreList()
        {
            List<Store> stores = _context.Stores.ToList();
            return View(stores);
        }

        public IActionResult Info(int id)
        {
            if (_context.Stores.FirstOrDefault(s => s.Id == id) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == id);
            return View(store);
        }
        [HttpGet]
        public IActionResult UpdateInfo(int id)
        {
            if (id == null) return NotFound();
            if (_context.Stores.FirstOrDefault(s => s.Id == id) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == id);
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
            if (store.LogoImageId== null && existStore.LogoImage!=null)
            {
                FileManager.DeleteFile(_env.WebRootPath, "uploads/stores",existStore.LogoImage);
                existStore.LogoImage = null;
            }
            //-------------------------------------------------------
            if (store.LogoImageFile != null)
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
            return RedirectToAction("info", "store", new { id = existStore.Id });

        }
    }
}
