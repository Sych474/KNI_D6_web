using KNI_D6_web.Model.Achievements;
using System.Collections.Generic;
using System.Linq;

namespace KNI_D6_web.ViewModels.Achievements
{
    public class AchievementsViewModel
    {
        public IEnumerable<IOrderedEnumerable<Achievement>> AchievementsInGroups { get; set; }
    }
}
