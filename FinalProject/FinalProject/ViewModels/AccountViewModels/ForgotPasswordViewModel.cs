using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace FinalProject.ViewModels.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Text_Required"), MaxLength(100),DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
