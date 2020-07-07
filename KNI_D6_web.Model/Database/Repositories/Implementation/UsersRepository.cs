using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class UsersRepository : BaseRepository, IUsersRepository
    {
        public UsersRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<User> FindFullUserByIdAsync(string id)
        {
            return context.Users
                .Include(x => x.ParameterValues).ThenInclude(pv => pv.Parameter)
                .Include(x => x.UserAchievements).ThenInclude(pv => pv.Achievement)
                .Include(x => x.UserEvents).ThenInclude(pv => pv.Event)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<User> FindUserWithAchievementsByIdAsync(string id)
        {
            return context.Users
                .Include(x => x.UserAchievements)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<List<User>> GetUsersByPositionAsync(UserPosition position)
        {
            return context.Users.Where(u => u.Position == position).ToListAsync();
        }

        public async Task<List<User>> GetUsersWithLinksAsync()
        {
            return await context.Users
                   .Include(u => u.UserAchievements)
                   .Include(u => u.ParameterValues)
                   .Include(u => u.UserEvents).ThenInclude(ue => ue.Event)
                   .ToListAsync();
        }

        public async Task UpdateUserAsync(User entity)
        {
            context.Users.Update(entity);
            await context.SaveChangesAsync();
        }
    }
}
