using KNI_D6_web.Model.Database.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Achievements
{
    public class AchievementsManager : IAchievementsManager
    {
        private readonly IAchievementsCalculator achievementsCalculator;
        private readonly IUserAchievementsRepository userAchievementsRepository;
        private readonly IAchievementsRepository achievementsRepository;

        public AchievementsManager(IAchievementsCalculator achievementsCalculator, IUserAchievementsRepository userAchievementsRepository, IAchievementsRepository achievementsRepository, IUsersRepository usersRepository)
        {
            this.achievementsCalculator = achievementsCalculator;
            this.userAchievementsRepository = userAchievementsRepository;
            this.achievementsRepository = achievementsRepository;
        }

        public async Task CheckAndUpdateСalculatedAchievementForUsers(IEnumerable<string> userIds, int achievementId)
        {
            foreach (var userId in userIds)
            {
                await CheckAndUpdateСalculatedAchievementForUser(userId, achievementId);
            }
        }

        public async Task CheckAndUpdateСalculatedAchievementsForUsers(IEnumerable<string> userIds, IEnumerable<int> achievementsIds)
        {
            foreach (var achievementId in achievementsIds)
            {
                await CheckAndUpdateСalculatedAchievementForUsers(userIds, achievementId);
            }
        }

        public async Task CheckAndUpdateСalculatedAchievementsForUser(string userId, IEnumerable<int> achievementIds)
        {
            foreach (var achievementId in achievementIds)
            {
                await CheckAndUpdateСalculatedAchievementForUser(userId, achievementId);
            }
        }

        public async Task CheckAndUpdateСalculatedAchievementForUser(string userId, int achievementId)
        {
            var achievement = await achievementsRepository.FindAchievementByIdAsync(achievementId);
            if (achievement != null && achievement.Parameter != null)
            {
                if (await achievementsCalculator.IsDone(achievementId, userId))
                    await userAchievementsRepository.AddUserAchievementAsync(achievementId, userId);   
            }
        }

    }
}
