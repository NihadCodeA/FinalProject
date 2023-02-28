using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.StoreViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using QRCoder;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace FinalProject.Controllers
{
    public class StoreController : Controller
    {
        private readonly Database _context;
        private readonly HttpContext _httpContext;
        private readonly IWebHostEnvironment _env;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public StoreController(Database context, IHttpContextAccessor httpContextAccessor, IStringLocalizer<SharedResource> localizer, IWebHostEnvironment env)
        {
            _context = context;
            _httpContext = httpContextAccessor.HttpContext;
            _env = env;
            _localizer = localizer;
        }

        public IActionResult Index(int storeId, int page = 1)
        {
            if (_context.Stores.FirstOrDefault(s => s.Id == storeId) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == storeId);
            var query = _context.Products.Include(pi => pi.ProductImages).Include(c=>c.Category).Where(p => p.StoreId == storeId).AsQueryable();
            
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
                Language= GetCurrentLanguage.CurrentLanguage(_httpContext),
                Localizer=_localizer,
            };
            return View(storeViewModel);
        }
        public IActionResult Info(int id)
        {
            if (_context.Stores.FirstOrDefault(s => s.Id == id) == null)
            {
                return NotFound();
            }
            Store store = _context.Stores.FirstOrDefault(s => s.Id == id);

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
            return View(store);
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
            List<Store> stores = _context.Stores.Include(p=>p.Products).OrderByDescending(x=>x.Id).ToList();
            return View(stores);
        }

    }
}
