using System.ComponentModel.DataAnnotations;

namespace FinalProject.Areas.Manage.ViewModels
{
    public class AdminRegisterViewModel
    {
        [Required(ErrorMessage = "yazılmalıdır"), StringLength(maximumLength: 60)]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "yazılmalıdır"), StringLength(maximumLength: 100), DataType(dataType: DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "yazılmalıdır"), DataType(dataType: DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "yazılmalıdır"), DataType(dataType: DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Təsdiqləmə şifrəsi ilə şifrə uyğunlaşmır!")]
        public string ConfirmPassword { get; set; }
    }
}
