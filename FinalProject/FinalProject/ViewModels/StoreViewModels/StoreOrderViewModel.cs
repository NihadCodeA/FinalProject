using FinalProject.Models;
using FinalProject.ViewModels.ProductViewModels;
namespace FinalProject.ViewModels.StoreViewModels
{
    public class StoreOrderViewModel
    {
        public int OrderId { get; set; }
        public string? AppUserId { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public AppUser AppUser { get; set; }
        public List<ProductCheckoutViewModel> CheckItems { get; set; }
    }
}
