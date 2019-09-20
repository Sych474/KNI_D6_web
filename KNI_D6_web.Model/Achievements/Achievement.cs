using System.Collections.Generic;

namespace KNI_D6_web.Model.Achievements
{
    public class Achievement
    {
        public int Id { get; set; }

        public int AchievementsGroupId { get; set; }

        public AchievementsGroup AchievementGroup { get; set; }

        public int NumberInGroup { get; set; } 

        public string Name { get; set; }

        public string Description { get; set; }

        public List<AchievementParameter> AchievementParameters { get; set; } = new List<AchievementParameter>();

        public List<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

        public AchievementType AchievementType { get; set; } = AchievementType.Custom;

        public int? AchievementValue { get; set; }

        public int? SemesterId { get; set; } = null;

        public Semester Semester { get; set; }
    }
}
