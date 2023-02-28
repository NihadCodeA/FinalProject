using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModels.HeaderViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using FinalProject.ViewModels.ProductViewModels;
using System.Diagnostics.Metrics;

namespace FinalProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public readonly Database _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly HttpContext _httpContext;
        public ProductController(UserManager<AppUser> userManager, Database context,
            IStringLocalizer<SharedResource> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
            _httpContext = httpContextAccessor.HttpContext;
        }



        public async Task<IActionResult> Detail(int productId)
        {
            if(productId==null) return NotFound();
            Product product = _context.Products.Include(s=>s.Store)
                .Include(pi=>pi.ProductImages).Include(c=>c.Category).FirstOrDefault(p=>p.Id == productId);
            if (product == null) return NotFound();
            Store store = _context.Stores.FirstOrDefault(s=>s.Id==product.StoreId);
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            //------------------------------------------------------------------------------
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItemViewModel basketItem = null;
            if (User.Identity.IsAuthenticated == false)
            {
                string basketItemStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                }

            }
            else
            {
                basketItems = _context.BasketItems.Where(x => x.AppUserId == appUser.Id).ToList();
                foreach (var item in basketItems)
                {
                    basketItem = new BasketItemViewModel
                    {
                        ProductId = (int)item.ProductId,
                        Count = item.Count,
                        Price = item.Price,
                        Discount = item.Discount,
                        Name = item.Name,
                        Weight = item.Weight,
                        Image = item.Image,
                    };
                    basketItemsVM.Add(basketItem);
                }
            }
            //---------------------------------------------------------------------------------
            HeaderViewModel headerVM = new HeaderViewModel
            {
                Store = store,
                Product = product,
                User = appUser,
                Language = GetCurrentLanguage.CurrentLanguage(_httpContext),
                Localizer = _localizer,
                RelatedProdcuts = _context.Products.Include(c => c.Category).Where(p => p.CategoryId == product.CategoryId).Take(5).ToList(),
                Categories8 = _context.Categories.Take(8).ToList(),
                Categories16 = _context.Categories.Skip(8).Take(8).ToList(),
                Categories24 = _context.Categories.Skip(16).Take(8).ToList(),
                Categories32 = _context.Categories.Skip(24).Take(8).ToList(),
                BasketItemViewModels = basketItemsVM,
            };
            return View(headerVM);
        }

        public async Task<IActionResult> ShopCart()
        {
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            //------------------------------------------------------------------------------
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItemViewModel basketItem = null;
            if (User.Identity.IsAuthenticated == false)
            {
                string basketItemStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                }

            }
            else
            {
                basketItems = _context.BasketItems.Where(x => x.AppUserId == appUser.Id).ToList();
                foreach (var item in basketItems)
                {
                    basketItem = new BasketItemViewModel
                    {
                        ProductId = (int)item.ProductId,
                        Count = item.Count,
                        Price = item.Price,
                        Discount = item.Discount,
                        Name = item.Name,
                        Weight = item.Weight,
                        Image = item.Image,
                    };
                    basketItemsVM.Add(basketItem);
                }
            }
            //---------------------------------------------------------------------------------
            return View(basketItemsVM);
        }

        public async Task<IActionResult> Checkout()
        {
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            //------------------------------------------------------------------------------
            List<ProductCheckoutViewModel> checkoutVMS = new List<ProductCheckoutViewModel>();
            ProductCheckoutViewModel checkoutVM = null;
            OrderViewModel orderVM = null;
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItemViewModel basketItem = null;
            if (User.Identity.IsAuthenticated == false)
            {
                string basketItemStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                    foreach (var item in basketItemsVM)
                    {
                        checkoutVM = new ProductCheckoutViewModel
                        {
                            Product = _context.Products.Include(pi => pi.ProductImages).FirstOrDefault(x => x.Id == item.ProductId),
                            Count = item.Count,

                        };
                        checkoutVMS.Add(checkoutVM);
                    }
                }

            }
            else
            {
                basketItems = _context.BasketItems.Include(p=>p.Product).Include(pi=>pi.Product.ProductImages).Where(x => x.AppUserId == appUser.Id).ToList();
                foreach (var item in basketItems)
                {
                        checkoutVM = new ProductCheckoutViewModel
                        {
                            Product = item.Product,
                            Count = item.Count,
                        };
                    checkoutVMS.Add(checkoutVM);
                }
            }
            //---------------------------------------------------------------------------------
            orderVM = new OrderViewModel
            {
                ChechoutItems = checkoutVMS,
            };
            return View(orderVM);
        }

        [HttpPost]
        public async Task<IActionResult> Order(OrderViewModel orderVM)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Checkout));
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<ProductCheckoutViewModel> checkoutItems = new List<ProductCheckoutViewModel>();
            OrderItem orderItem = null;
            List<BasketItem> memberBasketItems = new List<BasketItem>();
            AppUser member = null;
            string basketItemStr = HttpContext.Request.Cookies["BasketItems"];
            double totalPrice = 0;
            if (User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            Order order = new Order
            {
                FullName = orderVM.FullName,
                Email = orderVM.Email,
                Country = orderVM.Country,
                City = orderVM.City,
                Address1 = orderVM.Address1,
                Address2 = orderVM.Address2,
                OtherNotes = orderVM.OtherNotes,
                PhoneNumber = orderVM.PhoneNumber,
                CreatedTime = DateTime.Now,
                AppUserId = member?.Id,
            };
            StoreOrderItem storeOrder = new StoreOrderItem ();
            storeOrder.AppUserId = member?.Id;
            if (member == null)
            {
                if (basketItemStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);

                    foreach (var item in basketItems)
                    {
                        Product product = _context.Products.Include(s=>s.Store).FirstOrDefault(x => x.Id == item.ProductId);
                        if (product == null) return NotFound();
                        orderItem = new OrderItem
                        {
                            Product= product,
                            Store = product.Store,
                            ProductName = product.Name,
                            CostPrice = product.CostPrice,
                            DiscountPercentage = product.DiscountPercentage,
                            Count = item.Count,
                            SalePrice = (product.SalePrice * ((1 - product.DiscountPercentage / 100)))
                        };
                        totalPrice += orderItem.SalePrice * orderItem.Count;
                        order.OrderItems.Add(orderItem);
                        storeOrder.OrderItems.Add(orderItem);
                        storeOrder.Store = product.Store;
                        storeOrder.StoreId = (int)product.StoreId;
                    }
                }
            }
            else
            {
                memberBasketItems = _context.BasketItems.Include(x => x.Product).Where(x => x.AppUserId == member.Id).ToList();
                foreach (var item in memberBasketItems)
                {
                    Product product = _context.Products.Include(s => s.Store).FirstOrDefault(x => x.Id == item.ProductId);
                    if (product == null) return NotFound();
                    orderItem = new OrderItem
                    {
                        Product = product,
                        Store = product.Store,
                        ProductName = product.Name,
                        CostPrice = product.CostPrice,
                        DiscountPercentage = product.DiscountPercentage,
                        Count = item.Count,
                        SalePrice = (product.SalePrice * ((1 - product.DiscountPercentage / 100)))
                    };
                    totalPrice += orderItem.SalePrice * orderItem.Count;
                    order.OrderItems.Add(orderItem);
                    storeOrder.OrderItems.Add(orderItem);
                    storeOrder.Store = product.Store;
                    storeOrder.StoreId = (int)product.StoreId;
                }
            }
            order.TotalPrice = totalPrice;
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction(nameof(Checkout));
        }

        public async Task<IActionResult> AddToBasket(int productId)
        {
            if (!_context.Products.Any(x => x.Id == productId)) return NotFound(); //404
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];
            Product product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == productId);
            AppUser member = null;
            if (User.Identity.IsAuthenticated)
            {
                member = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            if (User.Identity.IsAuthenticated == false)
            {
                if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
                basketItem = basketItems.FirstOrDefault(x => x.ProductId == productId);
                if (basketItem != null) basketItem.Count++;
                else
                {
                    basketItem = new BasketItemViewModel
                    {
                        ProductId = productId,
                        Count = 1,
                        Price = product.SalePrice,
                        Discount = product.DiscountPercentage,
                        Name = product.Name,
                        Weight=product.Weight,
                        Image = product.ProductImages.FirstOrDefault(x => x.IsPoster == true)?.Image
                    };
                    basketItems.Add(basketItem);
                }
            }
            else
            {
                basketItem = new BasketItemViewModel
                {
                    ProductId = productId,
                    Count = 1,
                    Price = product.SalePrice,
                    Discount = product.DiscountPercentage,
                    Name = product.Name,
                    Weight = product.Weight,
                    Image = product.ProductImages.FirstOrDefault(x => x.IsPoster == true)?.Image
                };
                basketItems.Add(basketItem);
            }
            basketItemsStr = JsonConvert.SerializeObject(basketItems);
            HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);
            }
            else
            {
                BasketItem memberBasketItem = _context.BasketItems.Include(x => x.Product)
                     .FirstOrDefault(x => x.AppUserId == member.Id && x.Product.Id == productId);

                if (memberBasketItem != null)
                {
                    memberBasketItem.Count++;
                }
                else
                {
                    memberBasketItem = new BasketItem
                    {
                        AppUserId = member.Id,
                        ProductId = productId,
                        Count = 1,
                        Price = product.SalePrice,
                        Discount = product.DiscountPercentage,
                        Name = product.Name,
                        Weight = product.Weight,
                        Image = product.ProductImages.FirstOrDefault(x => x.IsPoster == true)?.Image
                    };
                    _context.BasketItems.Add(memberBasketItem);
                }
                _context.SaveChanges();
            }
            return Ok();
        }

        public async Task<IActionResult> GetBasket()
        {
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            //------------------------------------------------------------------------------
            List<BasketItemViewModel> basketItemsVM = new List<BasketItemViewModel>();
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItemViewModel basketItem = null;
            if (User.Identity.IsAuthenticated==false)
            {
                string basketItemStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemStr != null)
                {
                    basketItemsVM = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                }

            }
            else
            {
                basketItems = _context.BasketItems.Where(x => x.AppUserId == appUser.Id).ToList();
                foreach (var item in basketItems)
                {
                    basketItem = new BasketItemViewModel
                    {
                        ProductId = (int)item.ProductId,
                        Count = item.Count,
                        Price = item.Price,
                        Discount = item.Discount,
                        Name = item.Name,
                        Weight = item.Weight,
                        Image = item.Image,
                    };
                    basketItemsVM.Add(basketItem);
                }
            }

            return PartialView("_BasketItemVMPartial",basketItemsVM);
        }

        public async Task<IActionResult> RemoveFromBasket(int productId)
        {
            AppUser appUser = new AppUser();
            if (User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            //------------------------------------------------------------------------------
            if (!_context.Products.Any(x => x.Id == productId)) return NotFound(); //404
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<BasketItem> basketItemsDB = new List<BasketItem>();
            BasketItemViewModel basketItem = null;

            string basketItemStr = HttpContext.Request.Cookies["BasketItems"];

            if (User.Identity.IsAuthenticated == false)
            {
                if (basketItemStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemStr);
                    basketItem = basketItems.FirstOrDefault(x => x.ProductId == productId);
                    if (basketItem.Count == 0)
                    {
                        basketItems.Remove(basketItem);
                    }
                    else
                    {
                        basketItem.Count--;
                    }
                }
                basketItemStr = JsonConvert.SerializeObject(basketItems);
                HttpContext.Response.Cookies.Append("BasketItems", basketItemStr);
            }
            else
            {
                BasketItem memberBasketItem = _context.BasketItems.Include(x => x.Product)
                         .FirstOrDefault(x => x.AppUserId == appUser.Id && x.Product.Id == productId);
                    basketItemsDB = _context.BasketItems.Where(x => x.AppUserId == appUser.Id).ToList();
                    foreach (var item in basketItems)
                    {
                        basketItem = new BasketItemViewModel
                        {
                            ProductId = (int)item.ProductId,
                            Count = item.Count,
                            Price = item.Price,
                            Discount = item.Discount,
                            Name = item.Name,
                            Weight = item.Weight,
                            Image = item.Image,
                        };
                        basketItems.Add(basketItem);
                    }
                    if (memberBasketItem.Count == 0)
                    {
                        basketItemsDB.Remove(memberBasketItem);
                    }
                    else
                    {
                        memberBasketItem.Count--;
                    }
                    _context.SaveChanges();
            }


            return Json(basketItems);
        }
    }
}
