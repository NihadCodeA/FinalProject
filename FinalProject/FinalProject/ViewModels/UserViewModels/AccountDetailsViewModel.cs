using FinalProject.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.UserViewModels
{
    public class AccountDetailsViewModel
    {
        public AppUser User { get; set; }
        [Required, StringLength(maximumLength: 60)]
        public string Fullname { get; set; }
        [StringLength(maximumLength: 25), DataType(dataType: DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
