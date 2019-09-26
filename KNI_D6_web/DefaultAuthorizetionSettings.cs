namespace KNI_D6_web
{
    public static class DefaultAuthorizetionSettings
    {
        public const int PasswordMinLength = 5;
        public const int PasswordMaxLength = 20;
        public const string PasswordErrorMessage = "Пароль должен содержать не менее 5 и не более 20 символов";

        public const int LoginMinLength = 3;
        public const int LoginMaxLength = 15;
        public const string LoginErrorMessage = "Логин должен содержать не менее 3 и не более 15 символов";
    }
}
