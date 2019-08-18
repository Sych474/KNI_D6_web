using Microsoft.AspNetCore.Mvc.Rendering;

namespace KNI_D6_web.ViewModels.Achievements
{
    public class CreateCustomAchievementViewModel
    {
        public string AchievementName { get; set; }

        public string AhievementDescription { get; set; }

        public SelectList AchievementGroups { get; set; }

        public int GroupId { get; set; }

        public int NumberInGroup { get; set; }
    }
}
