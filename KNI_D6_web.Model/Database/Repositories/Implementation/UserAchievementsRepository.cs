using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class UserAchievementsRepository : BaseRepository, IUserAchievementsRepository
    {
        public UserAchievementsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task AddUserAchievementAsync(int achievementId, string userId)
        {
            if (!context.UserAchievements.Any(ua => ua.UserId == userId && ua.AchievementId == achievementId))
            {
                var userAchievement = new UserAchievement()
                {
                    UserId = userId,
                    AchievementId = achievementId
                };
                context.UserAchievements.Add(userAchievement);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveUserAchievementAsync(int achievementId, string userId)
        {
            var userAchievement = await context.UserAchievements.FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId);
            if (userAchievement != null)
            {
                context.UserAchievements.Remove(userAchievement);
                await context.SaveChangesAsync();
            }
        }
    }
}
