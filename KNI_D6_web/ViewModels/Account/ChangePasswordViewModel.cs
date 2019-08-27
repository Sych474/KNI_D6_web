using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [StringLength(100, ErrorMessage = "Пароль должен иметь не менее 5 символов", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [StringLength(100, ErrorMessage = "Пароль должен иметь не менее 5 символов", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [StringLength(100, ErrorMessage = "Пароль должен иметь не менее 5 символов", MinimumLength = 5)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают!")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; }
    }
}
