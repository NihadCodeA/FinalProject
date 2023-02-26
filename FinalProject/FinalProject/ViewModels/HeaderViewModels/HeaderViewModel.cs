using FinalProject.Models;
using Microsoft.Extensions.Localization;

namespace FinalProject.ViewModels.HeaderViewModels
{
    public class HeaderViewModel
    {
        public Store? Store { get; set; }
        public Product? Product { get; set; }
        public List<Product>? RelatedProdcuts { get; set; }
        public AppUser? User { get; set; }
        public string Language { get; set; }
        public IStringLocalizer Localizer { get; set; }
        public List<Category> Categories8 { get; set; }
        public List<Category> Categories16 { get; set; }
        public List<Category> Categories24 { get; set; }
        public List<Category> Categories32 { get; set; }
    }
}
