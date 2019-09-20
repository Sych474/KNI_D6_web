using KNI_D6_web.Model.Achievements;

namespace KNI_D6_web.ViewModels.Components.AchievementsProgress
{
    public class AchievementProgressViewModel
    {
        public int AchievementId { get; set; }

        public string AchievementName { get; set; }

        public string AchievementDescription { get; set; }

        public int? AchievementValue { get; set; }

        public int NumberInGroup { get; set; }

        public int? LinkedParameterValue { get; set; }

        public bool IsReceived { get; set; }

        public AchievementType AchievementType { get; set; }
    }
}
