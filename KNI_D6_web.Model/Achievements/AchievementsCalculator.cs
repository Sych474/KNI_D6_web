using KNI_D6_web.Model.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Achievements
{
    public class AchievementsCalculator : IAchievementsCalculator
    {
        private ApplicationDbContext dbContext;

        public AchievementsCalculator(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> IsDone(int achievementId, string userId)
        {
            var result = false;

            var achievement = await dbContext.Achievements
                .Include(a => a.AchievementParameters).ThenInclude(ap => ap.Parameter)
                .FirstOrDefaultAsync(a => a.Id == achievementId);

            if (achievement == null)
                throw new AchievementCalculatorException($"achievement with id {achievementId} not found");

            var user = await dbContext.Users
                .Include(u => u.ParameterValues)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new AchievementCalculatorException($"user with id {userId} not found");

            switch (achievement.AchievementType)
            {
                case AchievementType.Custom:
                    result = false; // manual achievement
                    break;
                case AchievementType.Calculated:
                    result = IsValueAchievementDone(achievement, user);
                    break;
                default:
                    break;
            }

            return result;
        }

        private bool IsValueAchievementDone(Achievement achievement, User user)
        {
            var achievementParameter = achievement.AchievementParameters.FirstOrDefault();

            if (!achievement.AchievementValue.HasValue)
                throw new AchievementCalculatorException("ValueAchievement should have NOT null value");

            if (achievement.AchievementParameters.Count != 1 || achievementParameter == null)
                throw new AchievementCalculatorException("In valueAchievement should be one parameter");

            var parameterValue = user.ParameterValues.FirstOrDefault(pv => pv.ParameterId == achievementParameter.ParameterId);

            return parameterValue.Value >= achievement.AchievementValue;
        }
    }
}
