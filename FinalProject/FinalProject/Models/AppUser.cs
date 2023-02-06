using Microsoft.AspNetCore.Identity;

namespace FinalProject.Models
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
    }
}
