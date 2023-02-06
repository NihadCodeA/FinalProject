using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace FinalProject.ViewModels.AccountViewModels
{
    public class MemberRegisterViewModel
    {
        [Required,StringLength(maximumLength:60)]
        public string Fullname { get; set; } 
        
        [Required,StringLength(maximumLength:100),DataType(dataType:DataType.EmailAddress)]
        public string Email { get; set; }
        
        [Required, DataType(dataType: DataType.Password)]
        public string Password { get; set; }
        
        [Required, DataType(dataType: DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
