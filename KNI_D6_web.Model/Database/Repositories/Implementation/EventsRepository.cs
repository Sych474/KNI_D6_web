using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories.Implementation
{
    public class EventsRepository : BaseRepository, IEventsRepository
    {
        public EventsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<int> AddEventAsync(Event entity)
        {
            context.Events.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        public Task<Event> FindEventByIdAsync(int id)
        {
            return context.Events.FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task<List<Event>> GetEventsAsync(int? semesterId = null)
        {
            Task<List<Event>> result;
            if (semesterId.HasValue)
                result = context.Events.Where(e => e.SemesterId == null || e.SemesterId == semesterId).ToListAsync();
            else
                result = context.Events.ToListAsync();
            return result;
        }

        public async Task RemoveEventByIdAsync(int id)
        {
            var entity = await context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (entity != null)
            {
                context.Events.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public Task UpdateEventAsync(Event entity)
        {
            context.Events.Update(entity);
            return context.SaveChangesAsync();
        }
    }
}
