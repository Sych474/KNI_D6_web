using KNI_D6_web.Model;
using KNI_D6_web.Model.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.ViewModels.Users
{
    public class UserDetailsViewModel
    {
        public bool IsAuthorizedUser { get; set; }
        public User User { get; set; }
    }
}
