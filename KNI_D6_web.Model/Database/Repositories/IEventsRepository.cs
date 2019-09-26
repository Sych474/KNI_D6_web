using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IEventsRepository
    {
        Task<List<Event>> GetEventsAsync(int? semesterId = null);

        Task<int> AddEventAsync(Event entity);

        Task<Event> FindEventByIdAsync(int id);

        Task UpdateEventAsync(Event entity);

        Task RemoveEventByIdAsync(int id);
    }
}
