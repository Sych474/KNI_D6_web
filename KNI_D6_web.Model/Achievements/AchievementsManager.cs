using KNI_D6_web.Model.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Achievements
{
    public class AchievementsManager : IAchievementsManager
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAchievementsCalculator achievementsCalculator;

        public AchievementsManager(ApplicationDbContext dbContext, IAchievementsCalculator achievementsCalculator)
        {
            this.dbContext = dbContext;
            this.achievementsCalculator = achievementsCalculator;
        }

        public async Task<int?> AddCalculatedAchievement(string name, string description, int value, int parameterId, int achievementGroupId, int numberInGroup)
        {
            int? result = null;

            if (!await dbContext.Achievements.AnyAsync(a => a.Name == name))
            {
                var achievement = new Achievement()
                {
                    Name = name,
                    Description = description,
                    AchievementValue = value,
                    AchievementType = AchievementType.Calculated,
                    AchievementsGroupId = achievementGroupId,
                    NumberInGroup = numberInGroup
                };

                await dbContext.Achievements.AddAsync(achievement);
                await dbContext.SaveChangesAsync();

                var achievementParameter = new AchievementParameter()
                {
                    AchievementId = achievement.Id,
                    ParameterId = parameterId
                };

                await dbContext.AchievementParameters.AddAsync(achievementParameter);
                await dbContext.SaveChangesAsync();

                result = achievement.Id;
            }

            return result;
        }

        public async Task<int?> AddCustomAchievement(string name, string description, int achievementGroupId, int numberInGroup)
        {
            int? result = null;

            if (!await dbContext.Achievements.AnyAsync(a => a.Name == name))
            {
                var achievement = new Achievement()
                {
                    Name = name,
                    Description = description,
                    AchievementValue = null,
                    AchievementType = AchievementType.Custom,
                    AchievementsGroupId = achievementGroupId,
                    NumberInGroup = numberInGroup
                };

                await dbContext.Achievements.AddAsync(achievement);
                await dbContext.SaveChangesAsync();

                result = achievement.Id;
            }

            return result;
        }

        public async Task<bool> AddAchievementToUser(int achievementId, string userId)
        {
            var userAchievement = new UserAchievement()
            {
                UserId = userId,
                AchievementId = achievementId
            };
            bool result;
            try
            {
                await dbContext.UserAchievements.AddAsync(userAchievement);
                await dbContext.SaveChangesAsync();
                result = true;
            }
            catch (System.Exception)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> RemoveUserAchievement(int achievementId, string userId)
        {
            var result = false;

            var user = await dbContext.Users
                .Include(u => u.UserAchievements)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var userAchievement = user.UserAchievements.FirstOrDefault(ua => ua.AchievementId == achievementId);
                if (userAchievement != null)
                {
                    dbContext.UserAchievements.Remove(userAchievement);
                    await dbContext.SaveChangesAsync();
                    result = true;
                }
            }
            return result;
        }

        public async Task CheckAndUpdateСalculatedAchievementForUsers(IEnumerable<string> userIds, int achievementId)
        {
            var achievement = await dbContext.Achievements.Include(a => a.AchievementParameters).FirstOrDefaultAsync(a => a.Id == achievementId);
            if (achievement != null && achievement.AchievementParameters.Any())
            {
                var parameterId = achievement.AchievementParameters.First().ParameterId;
                foreach (var userId in userIds)
                {
                    if (await achievementsCalculator.IsDone(achievementId, userId))
                        await AddAchievementToUser(achievementId, userId);
                }
            }
        }

        public async Task CheckAndUpdateСalculatedAchievementsForUsers(IEnumerable<string> userIds, IEnumerable<int> achievementsIds)
        {
            foreach (var achievementId in achievementsIds)
            {
                await CheckAndUpdateСalculatedAchievementForUsers(userIds, achievementId);
            }
        }
    }
}
