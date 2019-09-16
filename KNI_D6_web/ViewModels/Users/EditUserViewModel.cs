using KNI_D6_web.Model;

namespace KNI_D6_web.ViewModels.Users
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }

        public string Login { get; set; }

        public UserRoles.UserRolesEnum UserRole { get; set; }

        public UserPosition UserPosition { get; set; }
    }
}
