using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class AppUser:IdentityUser
    {
        [StringLength(maximumLength: 60)]
        public string? FullName { get; set; }
        public string? Address { get; set; }

        public List<BasketItem>? BasketItems { get; set; }

    }
}
