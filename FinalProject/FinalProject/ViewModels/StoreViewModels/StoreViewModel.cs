using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.Extensions.Localization;

namespace FinalProject.ViewModels.StoreViewModels
{
    public class StoreViewModel
    {
        public Store Store { get; set; }
        public PaginatedList<Product> Products { get; set; }
        public string Language { get; set; }
        public IStringLocalizer Localizer { get; set; }
    }
}
