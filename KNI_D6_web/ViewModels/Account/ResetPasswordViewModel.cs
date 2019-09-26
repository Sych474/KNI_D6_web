using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Поле Email должно быть заполнено")]
        [EmailAddress(ErrorMessage = "Введите корректный Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Пароль должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
