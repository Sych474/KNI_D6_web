﻿using KNI_D6_web.ViewModels.Components.AchievementsProgress;
using System.Collections.Generic;

namespace KNI_D6_web.ViewModels.Users
{
    public class ManageAchievementsViewModel
    {
        public IEnumerable<AchievementProgressViewModel> Achievements { get; set; }

        public string Login { get; set; }

        public string UserId { get; set; }
    }
}
