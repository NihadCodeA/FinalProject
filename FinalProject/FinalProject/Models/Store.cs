using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        [StringLength(maximumLength: 100)]
        public string? StoreName { get; set; }
        [StringLength(maximumLength: 25)]
        public string PhoneNumber1 { get; set; }
        [StringLength(maximumLength: 25)]
        public string? PhoneNumber2 { get; set; }
        [StringLength(maximumLength: 25)]
        public string? PhoneNumber3 { get; set; }
        [StringLength(maximumLength: 25)]
        public string? PhoneNumber4 { get; set; }
        [StringLength(maximumLength: 1000)]
        public string? Description { get; set; }
        public string? LogoImage { get; set; }
        [NotMapped]
        public int? LogoImageId { get; set; }
        [NotMapped]
        public IFormFile? LogoImageFile { get; set; }

        public List<Product>? Products { get; set; }

        public int? ViewCount { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsBanned { get; set; } = false;
    }
}
