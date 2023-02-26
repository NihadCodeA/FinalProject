using FinalProject.Models;
using Microsoft.Extensions.Localization;

namespace FinalProject.ViewModels.HomeViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Category> Categories { get; set; }
        public List<Product> PopularProducts { get; set; }
        public List<Product> DiscountedProducts { get; set; }
        public IStringLocalizer Localizer { get; set; }
        public string Lang { get; set; }
    }
}
