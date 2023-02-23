using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.AccountViewModels
{
    public class StoreRegisterViewModel
    {
        [Required(ErrorMessage = "Text_Required"), StringLength(maximumLength: 60)]
        public string Storename { get; set; }

        [Required(ErrorMessage = "Text_Required"), StringLength(maximumLength: 100), DataType(dataType: DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Text_Required"), StringLength(maximumLength:25), DataType(dataType: DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Text_Required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Text_Required"), DataType(dataType: DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Text_Required"), DataType(dataType: DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
