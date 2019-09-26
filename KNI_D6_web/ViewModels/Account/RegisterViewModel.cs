using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле Login должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.LoginMaxLength, ErrorMessage = DefaultAuthorizetionSettings.LoginErrorMessage, MinimumLength = DefaultAuthorizetionSettings.LoginMinLength)]
        public string Login { get; set; }

        [EmailAddress(ErrorMessage = "Введите корректный Email")]
        [Required(ErrorMessage = "Поле Email должно быть заполнено")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Пароль должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле Подтверждение пароля должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
