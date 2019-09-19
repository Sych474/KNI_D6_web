using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Achievements
{
    public interface IAchievementsManager
    {
        Task<bool> AddAchievementToUser(int achievementId, string userId);
        Task<int?> AddCalculatedAchievement(string name, string description, int value, int parameterId, int achievementGroupId, int numberInGroup, int? semesterId);
        Task<int?> AddCustomAchievement(string name, string description, int achievementGroupId, int numberInGroup, int? semesterId);
        Task CheckAndUpdateСalculatedAchievementForUser(string userId, int achievementId);
        Task CheckAndUpdateСalculatedAchievementForUsers(IEnumerable<string> userIds, int achievementId);
        Task CheckAndUpdateСalculatedAchievementsForUser(string userId, IEnumerable<int> achievementIds);
        Task CheckAndUpdateСalculatedAchievementsForUsers(IEnumerable<string> userIds, IEnumerable<int> achievementsIds);
        Task<bool> RemoveUserAchievement(int achievementId, string userId);
    }
}