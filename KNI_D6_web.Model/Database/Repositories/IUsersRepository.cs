using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IUsersRepository
    {
        Task<User> FindFullUserByIdAsync(string id);

        Task<User> FindUserWithAchievementsByIdAsync(string id);

        Task UpdateUserAsync(User entity);

        Task<List<User>> GetUsersByPositionAsync(UserPosition position);

        Task<List<User>> GetUsersWithLinksAsync();
    }
}
