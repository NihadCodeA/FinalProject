using FinalProject.Helpers;
using FinalProject.Models;

namespace FinalProject.ViewModels.ProductViewModels
{
    public class ProductViewModel
    {
        public Store Store { get; set; }
        public PaginatedList<Product> Products { get; set; }
    }
}
