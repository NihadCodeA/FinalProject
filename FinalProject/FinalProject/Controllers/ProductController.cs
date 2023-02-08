using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class ProductController : Controller
    {
        public readonly Database _context;
        public readonly IWebHostEnvironment _env;
        public ProductController(Database context,IWebHostEnvironment env)
        {
            _context = context;
            _env= env;
        }
        public IActionResult Index(int page =1)
        {
            int id = Int32.Parse(Request.Cookies["StoreId"]);
            
            Store store = _context.Stores.FirstOrDefault(x => x.Id == id);
            if (store == null) return NotFound();
            var query = _context.Products.Include(s=>s.Store)
                .Include(pi =>pi.ProductImages).Where(x=>x.StoreId==id).AsQueryable();
            var pagenatedProducts = PaginatedList<Product>.Create(query, 15, page);

            ProductViewModel productVM= new ProductViewModel
            {
                Store=store,
                Products=pagenatedProducts,
            };

            return View(productVM);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewData["StoreId"] = Request.Cookies["StoreId"];
            int id = Int32.Parse(Request.Cookies["StoreId"]);
            Store store = _context.Stores.FirstOrDefault(x => x.Id == id);
            if (store == null) return NotFound();
            var query = _context.Products.Include(s => s.Store);
            ViewData["StoreName"] = store.StoreName ?? "";
            return View();
        }
        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (product == null) return NotFound();
            ViewData["StoreId"] = Request.Cookies["StoreId"];
            int id = Int32.Parse(Request.Cookies["StoreId"]);
            Store store = _context.Stores.FirstOrDefault(x => x.Id == id);
            if (store == null) return NotFound();
            if (!ModelState.IsValid) return View();
            product.StoreId = id;
            product.CreatedTime = DateTime.Now;
            if (product.PosterImgFile != null)
            {
                if (product.PosterImgFile.ContentType != "image/png" && product.PosterImgFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFiles", "Ancaq png ve ya jpeg (jpg) formatinda olan sekilleri yukleye bilersiniz!");
                    return View();
                }
                if (product.PosterImgFile.Length > 3145728)
                {
                    ModelState.AddModelError("ImageFiles", "Seklin olcusu 3mb-den cox ola bilmez!");
                    return View();
                }
                ProductImages bookImage = new ProductImages
                {
                    Product = product,
                    Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", product.PosterImgFile),
                    IsPoster = true
                };
                _context.ProductImages.Add(bookImage);
            }
            if (product.ImageFiles != null)
            {
                foreach (IFormFile imageFile in product.ImageFiles)
                {
                    if (imageFile.ContentType != "image/png" && imageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFiles", "Ancaq png ve ya jpeg (jpg) formatinda olan sekilleri yukleye bilersiniz!");
                        return View();
                    }
                    if (imageFile.Length > 3145728)
                    {
                        ModelState.AddModelError("ImageFiles", "Seklin olcusu 3mb-den cox ola bilmez!");
                        return View();
                    }
                    ProductImages bookImage = new ProductImages
                    {
                        Product = product,
                        Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", product.PosterImgFile),
                        IsPoster = false
                    };
                    _context.ProductImages.Add(bookImage);
                }
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index","Product");
        }

    }
}
