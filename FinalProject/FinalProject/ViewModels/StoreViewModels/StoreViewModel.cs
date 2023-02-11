using FinalProject.Helpers;
using FinalProject.Models;

namespace FinalProject.ViewModels.StoreViewModels
{
    public class StoreViewModel
    {
        public Store Store { get; set; }
        public PaginatedList<Product> Products { get; set; }
    }
}
