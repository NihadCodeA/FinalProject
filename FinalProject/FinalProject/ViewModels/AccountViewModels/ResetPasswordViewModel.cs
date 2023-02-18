using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
        [Required,DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
