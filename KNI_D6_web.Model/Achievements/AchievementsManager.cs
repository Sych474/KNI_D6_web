﻿using KNI_D6_web.Model.Database;
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

        public async Task<int?> AddCalculatedAchievement(string name, string description, int value, int parameterId, int achievementGroupId, int numberInGroup, int? semesterId)
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
                    NumberInGroup = numberInGroup,
                    SemesterId = semesterId,
                    ParameterId = parameterId
                };

                await dbContext.Achievements.AddAsync(achievement);
                await dbContext.SaveChangesAsync();

                result = achievement.Id;
            }

            return result;
        }

        public async Task<int?> AddCustomAchievement(string name, string description, int achievementGroupId, int numberInGroup, int? semesterId)
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
                    NumberInGroup = numberInGroup,
                    SemesterId = semesterId
                };

                await dbContext.Achievements.AddAsync(achievement);
                await dbContext.SaveChangesAsync();

                result = achievement.Id;
            }

            return result;
        }

        public async Task AddAchievementToUser(int achievementId, string userId)
        {
            if (!dbContext.UserAchievements.Any(ua => ua.UserId == userId && ua.AchievementId == achievementId))
            {
                var userAchievement = new UserAchievement()
                {
                    UserId = userId,
                    AchievementId = achievementId
                };
                await dbContext.UserAchievements.AddAsync(userAchievement);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveUserAchievement(int achievementId, string userId)
        {
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
                }
            }
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

        public async Task CheckAndUpdateСalculatedAchievementForUser(string userId, int achievementId)
        {
            var achievement = await dbContext.Achievements.Include(a => a.Parameter).FirstOrDefaultAsync(a => a.Id == achievementId);
            if (achievement != null && achievement.Parameter != null)
            {
                if (await achievementsCalculator.IsDone(achievementId, userId))
                        await AddAchievementToUser(achievementId, userId);   
            }
        }

        public async Task CheckAndUpdateСalculatedAchievementsForUser(string userId, IEnumerable<int> achievementIds)
        {
            foreach (var achievementId in achievementIds)
            {
                await CheckAndUpdateСalculatedAchievementForUser(userId, achievementId);
            }
        }

        public async Task<int?> AddAchievement(Achievement achievement)
        {
            int? result = null;

            if (!await dbContext.Achievements.AnyAsync(a => a.Name == achievement.Name))
            {
                await dbContext.Achievements.AddAsync(achievement);
                await dbContext.SaveChangesAsync();

                result = achievement.Id;
            }
            return result;
        }
    }
}
