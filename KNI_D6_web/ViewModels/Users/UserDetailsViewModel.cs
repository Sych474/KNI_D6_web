using KNI_D6_web.Model;
using System.Collections.Generic;

namespace KNI_D6_web.ViewModels.Users
{
    public class UserDetailsViewModel
    {
        public bool IsAuthorizedUser { get; set; }

        public User User { get; set; }

        public IEnumerable<UserDetailsEventViewModel> EventsViewModels { get; set; }
    }
}
