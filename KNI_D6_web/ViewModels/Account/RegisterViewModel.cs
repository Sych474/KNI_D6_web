using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле Login должно быть заполнено")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле Email должно быть заполнено")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Пароль должно быть заполнено")]
        [StringLength(100, ErrorMessage = "Пароль должен иметь не менее 5 символов", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле Подтверждение пароля должно быть заполнено")]
        [StringLength(100, ErrorMessage = "Пароль должен иметь не менее 5 символов", MinimumLength = 5)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
