using System.Collections.Generic;
using System.Linq;

namespace KNI_D6_web.ViewModels.AchievementsProgress
{
    public class AchievementsProgressViewModel
    {
        public IEnumerable<IOrderedEnumerable<AchievementProgressViewModel>> AchievementsProgressInGroups { get; set; }
    }
}
