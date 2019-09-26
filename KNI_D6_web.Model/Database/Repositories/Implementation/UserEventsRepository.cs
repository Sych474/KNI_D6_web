using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class UserEventsRepository : BaseRepository, IUserEventsRepository
    {
        public UserEventsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task AddUserEventAsync(UserEvent entity)
        {
            context.UserEvents.Add(entity);
            return context.SaveChangesAsync();
        }

        public Task<UserEvent> FindUserEventAsync(string userId, int eventId)
        {
            return context.UserEvents.FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);
        }

        public async Task RemoveUserEventAsync(string userId, int eventId)
        {
            var entity = await context.UserEvents.FirstOrDefaultAsync(ue => ue.UserId == userId && ue.EventId == eventId);
            if (entity != null)
            {
                context.UserEvents.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}
