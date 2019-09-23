using KNI_D6_web.Model.Achievements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IAchievementsGroupsRepository
    {
        Task<List<AchievementsGroup>> GetAchievementsGroupsAsync();

        Task<int> AddAchievementsGroupAsync(AchievementsGroup entity);

        Task UpdateAchievementsGroupAsync(AchievementsGroup entity);

        Task RemoveAchievementsGroupByIdAsync(int id);

        Task<AchievementsGroup> FindAchievementsGroupByIdAsync(int id);
    }
}
