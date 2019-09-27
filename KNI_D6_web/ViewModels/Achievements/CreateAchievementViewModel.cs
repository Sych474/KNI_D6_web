﻿using KNI_D6_web.Model.Achievements;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KNI_D6_web.ViewModels.Achievements
{
    public class CreateAchievementViewModel
    {
        public string AchievementName { get; set; }

        public string AhievementDescription { get; set; }

        public int AchievementValue { get; set; }

        public int? ParameterId { get; set; }

        public int GroupId { get; set; }

        public int NumberInGroup { get; set; }

        public int? SemesterId { get; set; }

        public AchievementType AchievementType { get; set; }
    }
}
