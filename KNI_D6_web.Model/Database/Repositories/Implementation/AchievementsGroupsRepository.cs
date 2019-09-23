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
            context.AchievementsGroups.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public Task<AchievementsGroup> FindAchievementsGroupByIdAsync(int id)
        {
            return context.AchievementsGroups.FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<List<AchievementsGroup>> GetAchievementsGroupsAsync()
        {
            return context.AchievementsGroups?.Include(ag => ag.Ahievements)?.ThenInclude(a => a.Parameter)?.ToListAsync();
        }

        public async Task RemoveAchievementsGroupByIdAsync(int id)
        {
            var entity = await context.AchievementsGroups.FirstOrDefaultAsync(a => a.Id == id);
            if (entity == null)
            {
                context.AchievementsGroups.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateAchievementsGroupAsync(AchievementsGroup entity)
        {
            context.AchievementsGroups.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
