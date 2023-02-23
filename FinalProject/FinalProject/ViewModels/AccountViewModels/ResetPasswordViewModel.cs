using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Text_Required"), DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Text_Required"), DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
