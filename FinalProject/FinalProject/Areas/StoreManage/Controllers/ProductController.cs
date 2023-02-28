using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.HeaderViewModels;
using FinalProject.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Cryptography.X509Certificates;

namespace FinalProject.Areas.StoreManage.Controllers
{
    [Area("StoreManage")]
    [Authorize(Roles = "Store")]
    public class ProductController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public readonly Database _context;
        public readonly IWebHostEnvironment _env;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly HttpContext _httpContext;
        public ProductController(UserManager<AppUser> userManager, Database context, IWebHostEnvironment env,
            IStringLocalizer<SharedResource> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _env = env;
            _localizer = localizer;
            _httpContext = httpContextAccessor.HttpContext;
        }
        public IActionResult Index(int page = 1,string? productName=null)
        {
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["PageName"] = "Products";
            ViewData["Localizer"] = _localizer;
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            ViewData["StoreName"] = store.StoreName ?? "";
            if (store == null) return NotFound();
            var query = _context.Products.Include(s => s.Store)
                .Include(pi => pi.ProductImages).Include(c=>c.Category).Where(x => x.StoreId == store.Id).AsQueryable();
            if (productName != null)
            {
                query = _context.Products.Where(p=>p.Name.Contains(productName)).Include(s => s.Store)
                .Include(pi => pi.ProductImages).Include(c=>c.Category).Where(x => x.StoreId == store.Id).AsQueryable();
            }
            var pagenatedProducts = PaginatedList<Product>.Create(query, 15, page);

            return View(pagenatedProducts);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewData["Localizer"] = _localizer;
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewBag.Categories = _context.Categories.Include(p => p.Products).ToList();
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();
            ViewData["StoreEmail"] = store.Email ?? "";
            ViewData["StoreName"] = store.StoreName ?? "";
            ViewData["StoreId"] = store.Id.ToString() ?? "1";
            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            ViewBag.Categories = _context.Categories.Include(p => p.Products).ToList();
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["Localizer"] = _localizer;
            if (product == null) return NotFound();
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            ViewData["StoreId"] = store.Id.ToString() ?? "1";
            if (store == null) return NotFound();
            if (!ModelState.IsValid) return View();
            product.StoreId = store.Id;
            product.CreatedTime = DateTime.Now;
            //-----------Error messages for languages----------------------------------------
            string lang = GetCurrentLanguage.CurrentLanguage(_httpContext);
            string ErrMessage = "";
            string zeroErrMessage = "";
            string StartingDateErrMessage1 = "";
            string StartingDateErrMessage2 = "";
            string StartingDateErrMessage3 = "";
            string EndingDateErrMessage1 = "";
            string EndingDateErrMessage2 = "";
            string DateErrMessage1 = "";
            string DateErrMessage2 = "";
            string posterImgFileErrMessage = "";
            string imageUploadErrMessage = "";
            string imageSizeErrMessage = "";
            if (lang == "az")
            {
                ErrMessage = "1-dən kiçik ədəd daxil edə bilmərsiz!";
                zeroErrMessage = "0-dan kiçik ədəd daxil edə bilmərsiz!";
                StartingDateErrMessage1 = "Son tarixi varsa, başlanğıc tarixidə olmalıdı!";
                StartingDateErrMessage2 = "Başlanğıc tarixi son tarixdən böyük ola bilməz!";
                StartingDateErrMessage3 = "Başlanğıc tarixi indiki tarixdən əvvəldə ola bilməz!";
                EndingDateErrMessage1 = "Başlanğıc tarixi varsa, son tarixdə olmalıdı!";
                EndingDateErrMessage2 = "Son tarix indiki tarixdən əvvəldə ola bilməz!";
                DateErrMessage1 = "Endirim faizi 0-dan böyük deyilsə endirim tarixlərin qeyd edə bilmərsiz!";
                DateErrMessage2 = "Endirim faizi 0-dan böyükdürsə endirim tarixlərin qeyd etməlisiniz!";
                posterImgFileErrMessage = "Məhsulun plakat(poster) şəkilin yüklənməsi məcburidir!";
                imageUploadErrMessage = "Ancaq png ve ya jpeg (jpg) formatinda olan sekilleri yukleye bilersiniz!";
                imageSizeErrMessage = "Şəklin ölçüsü 3mb-dən çox ola bilməz!";
            }
            else
            {
                ErrMessage = "You cannot enter a number less than 1!";
                zeroErrMessage = "You cannot enter a number less than 0!";
                StartingDateErrMessage1 = "If there is an end date, it should be on the start date!";
                StartingDateErrMessage2 = "The start date cannot be greater than the end date!";
                StartingDateErrMessage3 = "The start date cannot be earlier than the current date!";
                EndingDateErrMessage1 = "If there is a start date, it should be on the end date!";
                EndingDateErrMessage2 = "The last date cannot be earlier than the current date!";
                DateErrMessage1 = "If the percentage of the discount is not greater than 0, you cannot record the discount dates!";
                DateErrMessage2 = "If the discount percentage is greater than 0, you should note the discount dates!";
                posterImgFileErrMessage = "It is required to upload a poster image of the product!";
                imageUploadErrMessage = "You can only upload images in PNG or JPEG (JPG) format!";
                imageSizeErrMessage = "The size of the image cannot be more than 3mb.";
            }
            //-------------------------------------------------------------------------
            if (product.Weight <= 0)
            {
                ModelState.AddModelError("Weight", ErrMessage);
                return View();
            }
            if (product.NetQuantity <= 0)
            {
                ModelState.AddModelError("NetQuantity", ErrMessage);
                return View();
            }

            if (product.SalePrice <= 0)
            {
                ModelState.AddModelError("SalePrice", ErrMessage);
                return View();
            }
            if (product.CostPrice <= 0)
            {
                ModelState.AddModelError("SalePrice", ErrMessage);
                return View();
            }
            if (product.DiscountPercentage < 0)
            {
                ModelState.AddModelError("DiscountPercentage", zeroErrMessage);
                return View();
            }
            if (product.DiscountPercentage > 0)
            {
                if (product.DiscountStartingDate == null && product.DiscountEndingDate != null)
                {
                    ModelState.AddModelError("DiscountStartingDate", StartingDateErrMessage1);
                    return View();
                }
                if (product.DiscountStartingDate != null && product.DiscountEndingDate == null)
                {
                    ModelState.AddModelError("DiscountEndingDate", EndingDateErrMessage1);
                    return View();
                }
                if (product.DiscountStartingDate == null && product.DiscountEndingDate == null)
                {
                    ModelState.AddModelError("DiscountEndingDate", DateErrMessage2);
                    ModelState.AddModelError("DiscountEndingDate", DateErrMessage2);
                    return View();
                }
                if (product.DiscountStartingDate != null && product.DiscountEndingDate != null &&
                    product.DiscountStartingDate >= product.DiscountEndingDate)
                {
                    ModelState.AddModelError("DiscountStartingDate", StartingDateErrMessage2);
                    return View();
                }
                if (product.DiscountStartingDate != null && product.DiscountStartingDate < DateTime.Now)
                {
                    ModelState.AddModelError("DiscountStartingDate", StartingDateErrMessage3);
                    return View();
                }
                if (product.DiscountEndingDate != null && product.DiscountEndingDate < DateTime.Now)
                {
                    ModelState.AddModelError("DiscountEndingDate", EndingDateErrMessage2);
                    return View();
                }
            }
            else if (product.DiscountPercentage == 0 && (product.DiscountStartingDate != null || product.DiscountEndingDate != null))
            {
                ModelState.AddModelError("DiscountStartingDate", DateErrMessage1);
                ModelState.AddModelError("DiscountEndingDate", DateErrMessage1);
                return View();
            }

            //--------PRODUCT DATE---------------------------------
            if (product.StartingDate == null && product.EndingDate != null)
            {
                ModelState.AddModelError("StartingDate", StartingDateErrMessage1);
            }
            if (product.StartingDate != null && product.EndingDate == null)
            {
                ModelState.AddModelError("EndingDate", EndingDateErrMessage1);
            }
            if (product.StartingDate != null && product.EndingDate != null && product.StartingDate >= product.EndingDate)
            {
                ModelState.AddModelError("StartingDate", StartingDateErrMessage2);
            }
            //------------------------------------------

            if (product.PosterImgFile != null)
            {
                if (product.PosterImgFile.ContentType != "image/png" && product.PosterImgFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterImgFile", imageUploadErrMessage);
                    return View();
                }
                if (product.PosterImgFile.Length > 3145728)
                {
                    ModelState.AddModelError("PosterImgFile", imageSizeErrMessage);
                    return View();
                }
                ProductImages productImage = new ProductImages
                {
                    Product = product,
                    Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", product.PosterImgFile),
                    IsPoster = true
                };
                _context.ProductImages.Add(productImage);
            }
            else
            {
                ModelState.AddModelError("PosterImgFile", posterImgFileErrMessage);
                return View();
            }

            if (product.ImageFiles != null)
            {
                foreach (IFormFile imageFile in product.ImageFiles)
                {
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", imageUploadErrMessage);
                        return View();
                    }
                    if (imageFile.Length > 3145728)
                    {
                        ModelState.AddModelError("ImageFiles", imageSizeErrMessage);
                        return View();
                    }
                    ProductImages productImage = new ProductImages
                    {
                        Product = product,
                        Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", imageFile),
                        IsPoster = false
                    };
                    _context.ProductImages.Add(productImage);
                }
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index", "Product", new { area = "StoreManage", page = 1 });
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.Categories = _context.Categories.Include(p => p.Products).ToList();
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["Localizer"] = _localizer;
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();
            ViewData["StoreEmail"] = store.Email ?? "";
            ViewData["StoreName"] = store.StoreName ?? "";
            ViewData["StoreId"] = store.Id.ToString() ?? "1";

            Product product = _context.Products.Include(pi => pi.ProductImages).Include(c => c.Category).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        public IActionResult Update(Product product)
        {
            ViewBag.Categories = _context.Categories.Include(p => p.Products).ToList();
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["Localizer"] = _localizer;
            if (!ModelState.IsValid) return View();

            Product existProduct = _context.Products.Include(pi => pi.ProductImages).Include(c=>c.Category).FirstOrDefault(x => x.Id == product.Id);
            if (existProduct == null) return NotFound();
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();
            //---------------------------------------------------------
            if (product.ProductImageIds != null)
            {
                existProduct.ProductImages.RemoveAll(x => !product.ProductImageIds.Contains(x.Id) && x.IsPoster == false);
            }
            else
            {
                existProduct.ProductImages.RemoveAll(x => x.IsPoster == false);
            }
            //-------------------------------------------------------
            //-----------Error messages for languages----------------------------------------
            string lang = GetCurrentLanguage.CurrentLanguage(_httpContext);
            string ErrMessage = "";
            string zeroErrMessage = "";
            string StartingDateErrMessage1 = "";
            string StartingDateErrMessage2 = "";
            string StartingDateErrMessage3 = "";
            string EndingDateErrMessage1 = "";
            string EndingDateErrMessage2 = "";
            string DateErrMessage1 = "";
            string DateErrMessage2 = "";
            string posterImgFileErrMessage = "";
            string imageUploadErrMessage = "";
            string imageSizeErrMessage = "";
            if (lang == "az")
            {
                ErrMessage = "1-dan kiçik ədəd daxil edə bilmərsiz!";
                zeroErrMessage = "0-dan kiçik ədəd daxil edə bilmərsiz!";
                StartingDateErrMessage1 = "Son tarixi varsa, başlanğıc tarixidə olmalıdı!";
                StartingDateErrMessage2 = "Başlanğıc tarixi son tarixdən böyük ola bilməz!";
                StartingDateErrMessage3 = "Başlanğıc tarixi indiki tarixdən əvvəldə ola bilməz!";
                EndingDateErrMessage1 = "Başlanğıc tarixi varsa, son tarixdə olmalıdı!";
                EndingDateErrMessage2 = "Son tarix indiki tarixdən əvvəldə ola bilməz!";
                DateErrMessage1 = "Endirim faizi 0-dan böyük deyilsə endirim tarixlərin qeyd edə bilmərsiz!";
                DateErrMessage2 = "Endirim faizi 0-dan böyükdürsə endirim tarixlərin qeyd etməlisiniz!";
                posterImgFileErrMessage = "Məhsulun plakat(poster) şəkilin yüklənməsi məcburidir!";
                imageUploadErrMessage = "Ancaq png ve ya jpeg (jpg) formatinda olan sekilleri yukleye bilersiniz!";
                imageSizeErrMessage = "Şəklin ölçüsü 3mb-dən çox ola bilməz!";
            }
            else
            {
                ErrMessage = "You cannot enter a number less than 1!";
                zeroErrMessage = "You cannot enter a number less than 0!";
                StartingDateErrMessage1 = "If there is an end date, it should be on the start date!";
                StartingDateErrMessage2 = "The start date cannot be greater than the end date!";
                StartingDateErrMessage3 = "The start date cannot be earlier than the current date!";
                EndingDateErrMessage1 = "If there is a start date, it should be on the end date!";
                EndingDateErrMessage2 = "The last date cannot be earlier than the current date!";
                DateErrMessage1 = "If the percentage of the discount is not greater than 0, you cannot record the discount dates!";
                DateErrMessage2 = "If the discount percentage is greater than 0, you should note the discount dates!";
                posterImgFileErrMessage = "It is required to upload a poster image of the product!";
                imageUploadErrMessage = "You can only upload images in PNG or JPEG (JPG) format!";
                imageSizeErrMessage = "The size of the image cannot be more than 3mb.";
            }
            //-------------------------------------------------------------------------
            if (product.SalePrice <= 0)
            {
                ModelState.AddModelError("SalePrice", ErrMessage);
                return View(existProduct);
            }
            if (product.CostPrice <= 0)
            {
                ModelState.AddModelError("SalePrice", ErrMessage);
                return View(existProduct);
            }
            if (product.DiscountPercentage < 0)
            {
                ModelState.AddModelError("DiscountPercentage", zeroErrMessage);
                return View(existProduct);
            }
            if (product.DiscountPercentage > 0)
            {
                if (product.DiscountStartingDate == null && product.DiscountEndingDate != null)
                {
                    ModelState.AddModelError("DiscountStartingDate", StartingDateErrMessage1);
                    return View(existProduct);
                }
                if (product.DiscountStartingDate != null && product.DiscountEndingDate == null)
                {
                    ModelState.AddModelError("DiscountEndingDate", EndingDateErrMessage1);
                    return View(existProduct);
                }
                if (product.DiscountStartingDate == null && product.DiscountEndingDate == null)
                {
                    ModelState.AddModelError("DiscountEndingDate", DateErrMessage2);
                    ModelState.AddModelError("DiscountEndingDate", DateErrMessage2);
                    return View(existProduct);
                }
                if (product.DiscountStartingDate != null && product.DiscountEndingDate != null && product.DiscountStartingDate >= product.DiscountEndingDate)
                {
                    ModelState.AddModelError("DiscountStartingDate", StartingDateErrMessage2);
                    return View(existProduct);
                }
                if (product.DiscountStartingDate != null && product.DiscountStartingDate < product.CreatedTime)
                {
                    ModelState.AddModelError("DiscountStartingDate", StartingDateErrMessage3);
                    return View(existProduct);
                }
                if (product.DiscountEndingDate != null && product.DiscountEndingDate < product.CreatedTime)
                {
                    ModelState.AddModelError("DiscountEndingDate", EndingDateErrMessage2);
                    return View(existProduct);
                }
            }
            else if (product.DiscountPercentage == 0 && (product.DiscountStartingDate != null || product.DiscountEndingDate != null))
            {
                ModelState.AddModelError("DiscountStartingDate", DateErrMessage1);
                ModelState.AddModelError("DiscountEndingDate", DateErrMessage1);
                return View(existProduct);
            }
            //--------------------------------------------------------------------------
            //--------PRODUCT DATE---------------------------------
            if (product.StartingDate == null && product.EndingDate != null)
            {
                ModelState.AddModelError("StartingDate", StartingDateErrMessage1);
                return View(existProduct);
            }
            if (product.StartingDate != null && product.EndingDate == null)
            {
                ModelState.AddModelError("EndingDate", EndingDateErrMessage1);
                return View(existProduct);
            }
            if (product.StartingDate != null && product.EndingDate != null && product.StartingDate >= product.EndingDate)
            {
                ModelState.AddModelError("StartingDate", StartingDateErrMessage2);
                return View(existProduct);
            }

            //------------------------------------------
            if (product.PosterImgFile != null)
            {
                if (product.PosterImgFile.ContentType != "image/png" && product.PosterImgFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("PosterImgFile", imageUploadErrMessage);
                    return View();
                }
                if (product.PosterImgFile.Length > 3145728)
                {
                    ModelState.AddModelError("PosterImgFile", imageSizeErrMessage);
                    return View();
                }
                if (existProduct.ProductImages.FirstOrDefault(x => x.IsPoster == true).Image != null)
                {
                    FileManager.DeleteFile(_env.WebRootPath, "uploads/products", existProduct.ProductImages.FirstOrDefault(x => x.IsPoster == true).Image);
                    existProduct.ProductImages.FirstOrDefault(x => x.IsPoster == true).Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", product.PosterImgFile);
                }
                else
                {
                    ProductImages productImage = new ProductImages
                    {
                        Product = existProduct,
                        Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", product.PosterImgFile),
                        IsPoster = true
                    };
                    _context.ProductImages.Add(productImage);
                }
            }
            if (product.ImageFiles != null)
            {
                foreach (IFormFile imageFile in product.ImageFiles)
                {
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", imageUploadErrMessage);
                        return View();
                    }
                    if (imageFile.Length > 3145728)
                    {
                        ModelState.AddModelError("ImageFiles", imageSizeErrMessage);
                        return View();
                    }
                    ProductImages productImage = new ProductImages
                    {
                        Product = existProduct,
                        Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", imageFile),
                        IsPoster = false
                    };
                    _context.ProductImages.Add(productImage);
                }
            }
            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.StorageTip = product.StorageTip;
            existProduct.Disclaimer = product.Disclaimer;
            existProduct.CostPrice = product.CostPrice;
            existProduct.SalePrice = product.SalePrice;
            existProduct.DiscountPercentage = product.DiscountPercentage;
            existProduct.IsAvaible = product.IsAvaible;
            existProduct.Shipping = product.Shipping;
            existProduct.Weight = product.Weight;
            existProduct.NetQuantity = product.NetQuantity;
            existProduct.Brand = product.Brand;
            existProduct.StartingDate = product.StartingDate;
            existProduct.EndingDate = product.EndingDate;
            existProduct.DiscountStartingDate = product.DiscountStartingDate;
            existProduct.DiscountEndingDate = product.DiscountEndingDate;
            existProduct.ProductCount = product.ProductCount;
            existProduct.ProductCode = product.ProductCode;
            
            var categoryId = product.CategoryId;
            var category = _context.Categories.Find(categoryId);
            existProduct.Category = category;

            _context.SaveChanges();
            return RedirectToAction("Index", "Product", new { area = "StoreManage", page = 1 });
        }

        public IActionResult Delete(int id)
        {
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();

            Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            if (product.ProductImages != null)
            {
                foreach (var productImage in product.ProductImages)
                {
                    FileManager.DeleteFile(_env.WebRootPath, "uploads/products", productImage.Image);
                }
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("Index", "Product", new { area = "StoreManage", page = 1 });
        }

        public IActionResult Orders(int page = 1)
        {
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["PageName"] = "Orders";
            ViewData["Localizer"] = _localizer;
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            ViewData["StoreName"] = store.StoreName ?? "";
            if (store == null) return NotFound();
            //var query = _context.Orders.Include(s => s.AppUser).Include(o => o.OrderItems.Where(x => x.StoreId == store.Id)).AsQueryable();
            var query = _context.Orders.Include(s => s.AppUser)
    .Include(o => o.OrderItems.Where(x => x.StoreId == store.Id))
    .Where(o => o.OrderItems.Any(oi => oi.StoreId == store.Id)).AsQueryable();
            var pagenatedOrders = PaginatedList<Order>.Create(query, 10, page);

            return View(pagenatedOrders);
        }

        public IActionResult OrderDetail(int orderId)
        {
            ViewData["Language"] = GetCurrentLanguage.CurrentLanguage(_httpContext);
            ViewData["PageName"] = "Orders";
            ViewData["Localizer"] = _localizer;
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            ViewData["StoreName"] = store.StoreName ?? "";
            List<OrderItem> storeOrders = _context.OrderItems.Include(o => o.Order).Include(p=>p.Product)
                .ThenInclude(pi=>pi.ProductImages).Where(s=>s.StoreId==store.Id && s.OrderId==orderId && s.Count>0).ToList();
            ViewData["StoreOrder"] = _context.Orders.Include(oi => oi.OrderItems.Where(s=>s.StoreId==store.Id).
            FirstOrDefault(x=>x.OrderId==orderId));
            return View(storeOrders);
        }
    }
}
