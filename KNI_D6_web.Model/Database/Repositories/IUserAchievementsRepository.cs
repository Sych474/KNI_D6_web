using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IUserAchievementsRepository
    {
        Task AddUserAchievementAsync(int achievementId, string userId);

        Task RemoveUserAchievementAsync(int achievementId, string userId);
    }
}
