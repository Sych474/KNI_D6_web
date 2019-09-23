using System.Collections.Generic;
using System.Threading.Tasks;
using KNI_D6_web.Model.Achievements;
using Microsoft.EntityFrameworkCore;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class AchievementsGroupsRepository : BaseRepository, IAchievementsGroupsRepository
    {
        public AchievementsGroupsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> AddAchievementsGroupAsync(AchievementsGroup entity)
        {
            context.AchievementGroups.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public Task<AchievementsGroup> FindAchievementsGroupByIdAsync(int id)
        {
            return context.AchievementGroups.FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<List<AchievementsGroup>> GetAchievementsGroupsAsync()
        {
            return context.AchievementGroups?.Include(ag => ag.Ahievements)?.ThenInclude(a => a.Parameter)?.ToListAsync();
        }

        public async Task RemoveAchievementsGroupByIdAsync(int id)
        {
            var entity = await context.AchievementGroups.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                context.AchievementGroups.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAchievementsGroupAsync(AchievementsGroup entity)
        {
            context.AchievementGroups.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
