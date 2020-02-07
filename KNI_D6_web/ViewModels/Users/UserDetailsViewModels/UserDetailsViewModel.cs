using KNI_D6_web.Model;
using System.Collections.Generic;

namespace KNI_D6_web.ViewModels.Users.UserDetailsViewModels
{
    public class UserDetailsViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public UserPosition Position { get; set; }

        public IEnumerable<UserDetailsParameterViewModel> Parameters { get; set; }

        public IEnumerable<UserDetailsEventViewModel> CurrentEvents { get; set; }

        public IEnumerable<UserDetailsEventViewModel> OldEvents { get; set; }

        public IEnumerable<UserDetailsAchievementViewModel> Achievements { get; set; }
    }
}
