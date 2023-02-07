using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class AppUser:IdentityUser
    {
        public bool? IsSuperAdmin { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsStore { get; set; }
        public bool? IsMember { get; set; }
        [StringLength(maximumLength: 60)]
        public string? FullName { get; set; }
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
        [StringLength(maximumLength: 500)]
        public string? Description { get; set; }
        public string? LogoImage { get; set; }
        [NotMapped]
        public IFormFile? LogoImageFile { get; set; }
        public string? BackgroundImage { get; set; }
        [NotMapped]
        public IFormFile? BackgroundImageFile { get; set; }

    }
}
