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
            //int id = Int32.Parse(Request.Cookies["StoreId"]);
            //Store store = _context.Stores.FirstOrDefault(x => x.Id == id);
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();
            var query = _context.Products.Include(s=>s.Store)
                .Include(pi =>pi.ProductImages).Where(x=>x.StoreId==store.Id).AsQueryable();
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
            //ViewData["StoreId"] = Request.Cookies["StoreId"];
            //int id = Int32.Parse(Request.Cookies["StoreId"]);
            //Store store = _context.Stores.FirstOrDefault(x => x.Id == id);
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
            if (product == null) return NotFound();
            //ViewData["StoreId"] = Request.Cookies["StoreId"];
            //int id = Int32.Parse(Request.Cookies["StoreId"]);
            //Store store = _context.Stores.FirstOrDefault(x => x.Id == id);
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
                ProductImages productImage = new ProductImages
                {
                    Product = product,
                    Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products", product.PosterImgFile),
                    IsPoster = true
                };
                _context.ProductImages.Add(productImage);
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
                    ProductImages productImage = new ProductImages
                    {
                        Product = product,
                        Image = FileManager.SaveFile(_env.WebRootPath, "uploads/products",imageFile),
                        IsPoster = false
                    };
                    _context.ProductImages.Add(productImage);
                }
            }
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index","Product");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            Store store = new Store();
            if (User.Identity.IsAuthenticated && User.IsInRole("Store"))
            {
                store = _context.Stores.FirstOrDefault(x => x.Email == User.Identity.Name);
            }
            if (store == null) return NotFound();
            ViewData["StoreEmail"] = store.Email ?? "";
            ViewData["StoreName"] = store.StoreName ?? "";
            ViewData["StoreId"] = store.Id.ToString() ?? "1";

            Product product = _context.Products.Include(pi=>pi.ProductImages).FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        public IActionResult Update(Product product)
        {
            if (!ModelState.IsValid) return View();

            Product existProduct= _context.Products.Include(pi=>pi.ProductImages).FirstOrDefault(x=>x.Id== product.Id);
            if (existProduct == null) return NotFound();

            //---------------------------------------------------------
            if (product.ProductImageIds!=null)
            {
                existProduct.ProductImages.RemoveAll(x => !product.ProductImageIds.Contains(x.Id) && x.IsPoster == false);
            }
            else
            {
                existProduct.ProductImages.RemoveAll(x => x.IsPoster == false);
            }
            //-------------------------------------------------------
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
                if (existProduct.ProductImages != null)
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
                        ModelState.AddModelError("ImageFiles", "Ancaq png ve ya jpeg (jpg) formatinda olan sekilleri yukleye bilersiniz!");
                        return View();
                    }
                    if (imageFile.Length > 3145728)
                    {
                        ModelState.AddModelError("ImageFiles", "Seklin olcusu 3mb-den cox ola bilmez!");
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
            existProduct.StorageTip= product.StorageTip;
            existProduct.Disclaimer = product.Disclaimer;
            existProduct.CostPrice= product.CostPrice;
            existProduct.SalePrice= product.SalePrice;
            existProduct.DiscountPercentage= product.DiscountPercentage;
            existProduct.IsAvaible= product.IsAvaible;
            existProduct.Type = product.Type;
            existProduct.Shipping = product.Shipping;
            existProduct.Weight = product.Weight;
            existProduct.NetQuantity = product.NetQuantity;
            existProduct.IngredientType = product.IngredientType;
            existProduct.Brand = product.Brand;
            existProduct.Width = product.Width;
            existProduct.Height = product.Height;
            existProduct.Length = product.Length;
            existProduct.DimensionType = product.DimensionType;
            existProduct.StartingDate = product.StartingDate;
            existProduct.EndingDate = product.EndingDate;
            _context.SaveChanges();
            return RedirectToAction("Index","Product");
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

            return RedirectToAction("Index","Product");
        }
    }
}
