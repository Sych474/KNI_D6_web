using KNI_D6_web.Model;
using KNI_D6_web.Model.Achievements;

namespace KNI_D6_web.ViewModels.Achievements
{
    public class EditAchievementViewModel
    {
        public int Id { get; set; }

        public int AchievementsGroupId { get; set; }

        public int NumberInGroup { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AchievementType AchievementType { get; set; } = AchievementType.Custom;

        public int? AchievementValue { get; set; }

        public int? ParameterId { get; set; }

        public int? SemesterId { get; set; }

        public EditAchievementViewModel()
        {
        }

        public EditAchievementViewModel(Achievement achievement)
        {
            Id = achievement.Id;
            AchievementsGroupId = achievement.AchievementsGroupId;
            NumberInGroup = achievement.NumberInGroup;
            Name = achievement.Name;
            Description = achievement.Description;
            AchievementType = achievement.AchievementType;
            AchievementValue = achievement.AchievementValue;
            ParameterId = achievement.ParameterId;
            SemesterId = achievement.SemesterId;
        }

        public bool UpdateAchievement(Achievement achievement)
        {
            var result = false;

            if (achievement.Id == Id)
            {
                achievement.AchievementsGroupId = AchievementsGroupId;
                achievement.NumberInGroup = NumberInGroup;
                achievement.Name = Name;
                achievement.Description = Description;
                achievement.AchievementType = AchievementType;
                achievement.AchievementValue = AchievementValue;
                achievement.SemesterId = SemesterId;
                achievement.ParameterId = ParameterId;
                result = true;
            }
            return result;
        }
    }
}
