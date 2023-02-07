using FinalProject.DAL;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class ProductController : Controller
    {
        public readonly Database _context;
        public ProductController(Database context)
        {
            _context = context;
        }
        public IActionResult Index(int page =1)
        {
            var query = _context.Products.Include(s=>s.Store).Include(pi =>pi.ProductImages).AsQueryable();
            var pagenatedProducts = PaginatedList<Product>.Create(query, 15, page);
            return View(pagenatedProducts);
        }
    }
}
