using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Achievements
{
    public interface IAchievementsManager
    {
        Task CheckAndUpdateСalculatedAchievementForUser(string userId, int achievementId);
        Task CheckAndUpdateСalculatedAchievementForUsers(IEnumerable<string> userIds, int achievementId);
        Task CheckAndUpdateСalculatedAchievementsForUser(string userId, IEnumerable<int> achievementIds);
        Task CheckAndUpdateСalculatedAchievementsForUsers(IEnumerable<string> userIds, IEnumerable<int> achievementsIds);
    }
}