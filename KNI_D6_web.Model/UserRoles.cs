using System;
using System.Collections.Generic;
using System.Text;

namespace KNI_D6_web.Model
{
    public static class UserRoles
    {
        public const string AdminRole = "Admin";
        public const string UserRoleName = "User";
        public static List<string> Roles => new List<string>() { AdminRole, UserRoleName };
    }
}
