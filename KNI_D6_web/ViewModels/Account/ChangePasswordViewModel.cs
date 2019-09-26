using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают!")]
        [DataType(DataType.Password)]
        public string NewPasswordConfirm { get; set; }
    }
}
