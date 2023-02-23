using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;
namespace FinalProject.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Text_Required"), StringLength(maximumLength: 100), DataType(dataType: DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Text_Required"), DataType(dataType: DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
