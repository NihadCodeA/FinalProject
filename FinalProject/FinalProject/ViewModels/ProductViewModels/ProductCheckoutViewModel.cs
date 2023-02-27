using FinalProject.Models;

namespace FinalProject.ViewModels.ProductViewModels
{
    public class ProductCheckoutViewModel
    {
        public Store Store { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}
