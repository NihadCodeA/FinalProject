using FinalProject.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.ProductViewModels
{
    public class OrderViewModel
    {
        public string? AppUserId { get; set; }
        [StringLength(maximumLength: 60)]
        public string FullName { get; set; }
        [StringLength(maximumLength: 100)]
        public string Country { get; set; }
        [StringLength(maximumLength: 100)]
        public string City { get; set; }
        [StringLength(maximumLength: 100), DataType(dataType: DataType.EmailAddress)]
        public string Email { get; set; }
        [StringLength(maximumLength: 25), DataType(dataType: DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [StringLength(maximumLength: 150)]
        public string Address1 { get; set; }
        [StringLength(maximumLength: 150)]
        public string Address2 { get; set; }
        [StringLength(maximumLength: 1500)]
        public string OtherNotes { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? CreatedTime { get; set; }
        public AppUser? AppUser { get; set; }
        public List<ProductCheckoutViewModel>? ChechoutItems { get; set; }
    }
}
