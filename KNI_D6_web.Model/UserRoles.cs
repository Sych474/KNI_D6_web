using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KNI_D6_web.Model
{
    public static class UserRoles
    {
        public enum UserRolesEnum
        {
            [Description(UserRoles.Admin), Display(Name = UserRoles.Admin)]
            Admin,

            [Description(UserRoles.Moderator), Display(Name = UserRoles.Moderator)]
            Moderator,

            [Description(UserRoles.User), Display(Name = UserRoles.User)]
            User
        }


        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string User = "User";
        public static List<string> Roles => new List<string>() { Admin, User, Moderator };
        public const string AdminAndModerator = "Admin, Moderator";

        public static UserRolesEnum GetMaximumRole(IEnumerable<string> roles)
        {
            var result = UserRolesEnum.User;

            if (roles.Contains(Admin))
                result = UserRolesEnum.Admin;
            else if (roles.Contains(Moderator))
                result = UserRolesEnum.Moderator;

            return result;
        }

        public static IEnumerable<string> GetAllRolesByMaximumRole(UserRolesEnum role)
        {
            var result = new List<string>() { User };
            switch (role)
            {
                case UserRolesEnum.Admin:
                    result.Add(Admin);
                    result.Add(Moderator);
                    break;
                case UserRolesEnum.Moderator:
                    result.Add(Moderator);
                    break;
                case UserRolesEnum.User:
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
