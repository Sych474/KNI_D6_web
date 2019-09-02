using System.ComponentModel.DataAnnotations;

namespace KNI_D6_web.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Поле Email должно быть заполнено")]
        [EmailAddress(ErrorMessage = "Введите корректный Email")]
        public string Email { get; set; }
    }
}
