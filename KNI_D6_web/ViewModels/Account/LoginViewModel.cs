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
        [StringLength(100, ErrorMessage = "Пароль должен иметь не менее 5 символов", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
