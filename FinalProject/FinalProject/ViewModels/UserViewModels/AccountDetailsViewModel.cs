using FinalProject.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.UserViewModels
{
    public class AccountDetailsViewModel
    {
        [Required(ErrorMessage = "Text_Required"), StringLength(maximumLength: 60)]
        public string Fullname { get; set; }
        [StringLength(maximumLength: 25), DataType(dataType: DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
