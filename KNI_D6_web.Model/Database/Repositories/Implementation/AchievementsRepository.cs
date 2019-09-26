using System.Collections.Generic;
using System.Threading.Tasks;
using KNI_D6_web.Model.Achievements;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class AchievementsRepository : BaseRepository, IAchievementsRepository
    {
        public AchievementsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> AddAchievementAsync(Achievement entity)
        {
            context.Achievements.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public Task<Achievement> FindAchievementByIdAsync(int id)
        {
            return context.Achievements
                .Include(a => a.AchievementGroup)
                .Include(a => a.Parameter)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Achievement>> GetAchievementsAsync()
        {
            return context.Achievements.ToListAsync();
        }

        public async Task RemoveAchievementByIdAsync(int id)
        {
            var achievement = await context.Achievements.FirstOrDefaultAsync(p => p.Id == id);
            if (achievement != null)
            {
                context.Achievements.Remove(achievement);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAchievementAsync(Achievement entity)
        {
            context.Achievements.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
