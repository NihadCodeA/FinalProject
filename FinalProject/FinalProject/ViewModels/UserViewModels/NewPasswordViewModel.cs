using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModels.UserViewModels
{
    public class NewPasswordViewModel
    {
        [Required(ErrorMessage = "Text_Required"), DataType(dataType: DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Text_Required"), DataType(dataType: DataType.Password)]
        public string NewPassword { get; set; }
    }
}
