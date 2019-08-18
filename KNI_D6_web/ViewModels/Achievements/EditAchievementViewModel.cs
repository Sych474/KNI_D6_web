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

        //now achievement can have one or zero parameter
        public int? AchievementParameterId { get; set; }

        public EditAchievementViewModel()
        {
        }

        public EditAchievementViewModel(Achievement achievement, int? achievementParameterId)
        {
            Id = achievement.Id;
            AchievementsGroupId = achievement.AchievementsGroupId;
            NumberInGroup = achievement.NumberInGroup;
            Name = achievement.Name;
            Description = achievement.Description;
            AchievementType = achievement.AchievementType;
            AchievementValue = achievement.AchievementValue;
            AchievementParameterId = achievementParameterId;
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
                result = true;
            }
            return result;
        }

        public AchievementParameter GetAchievementParameter()
        {
            AchievementParameter result = null;
            if (AchievementParameterId.HasValue)
            {
                result = new AchievementParameter()
                {
                    AchievementId = Id,
                    ParameterId = AchievementParameterId.Value
                };
            }
            return result;
        }
    }
}
