using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.AccountViewModels
{
    public class StoreRegisterViewModel
    {
        [Required, StringLength(maximumLength: 60)]
        public string Storename { get; set; }

        [Required, StringLength(maximumLength: 100), DataType(dataType: DataType.EmailAddress)]
        public string Email { get; set; }

        [Required,DataType(dataType: DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required, DataType(dataType: DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(dataType: DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
