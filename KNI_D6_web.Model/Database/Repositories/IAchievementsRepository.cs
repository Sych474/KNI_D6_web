using KNI_D6_web.Model.Achievements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IAchievementsRepository
    {
        Task<int> AddAchievementAsync(Achievement entity);

        Task<Achievement> FindAchievementByIdAsync(int id);

        Task UpdateAchievementAsync(Achievement entity);

        Task RemoveAchievementByIdAsync(int id);

        Task<List<Achievement>> GetAchievementsAsync();

    }
}
