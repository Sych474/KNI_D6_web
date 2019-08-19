using System.Collections.Generic;
using System.ComponentModel;

namespace KNI_D6_web.Model
{
    public static class UserRoles
    {
        public enum UserRolesEnum
        {
            [Description(UserRoles.Admin)]
            Admin,

            [Description(UserRoles.Moderator)]
            Moderator,

            [Description(UserRoles.User)]
            User
        }


        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string User = "User";
        public static List<string> Roles => new List<string>() { Admin, User };
        public const string AdminAndModerator = "Admin, Moderator";
    }
}
