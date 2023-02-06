using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;
namespace FinalProject.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {

        [Required, StringLength(maximumLength: 100), DataType(dataType: DataType.EmailAddress)]
        public string Email { get; set; }

        [Required, DataType(dataType: DataType.Password)]
        public string Password { get; set; }
    }
}
