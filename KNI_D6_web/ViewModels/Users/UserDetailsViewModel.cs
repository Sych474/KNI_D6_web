using KNI_D6_web.Model;
using KNI_D6_web.ViewModels.Components.AchievementsProgress;
using KNI_D6_web.ViewModels.Visits;
using System.Collections.Generic;

namespace KNI_D6_web.ViewModels.Users
{
    public class UserDetailsViewModel
    {
        public User User { get; set; }

        public IEnumerable<EventVisitViewModel> EventVisits { get; set; }

        public IEnumerable<AchievementProgressViewModel> UserAchievements { get; set; }
    }
}
