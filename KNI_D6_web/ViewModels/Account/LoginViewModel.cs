using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле Login должно быть заполнено")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле Пароль должно быть заполнено")]
        [StringLength(DefaultAuthorizetionSettings.PasswordMaxLength, ErrorMessage = DefaultAuthorizetionSettings.PasswordErrorMessage, MinimumLength = DefaultAuthorizetionSettings.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
