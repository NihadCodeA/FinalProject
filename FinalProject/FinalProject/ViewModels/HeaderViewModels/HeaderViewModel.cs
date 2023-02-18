using FinalProject.Models;
using Microsoft.Extensions.Localization;

namespace FinalProject.ViewModels.HeaderViewModels
{
    public class HeaderViewModel
    {
        public Store? Store { get; set; }
        public Product? Product { get; set; }
        public AppUser? User { get; set; }
        public string Language { get; set; }
        public IStringLocalizer Localizer { get; set; }

    }
}
