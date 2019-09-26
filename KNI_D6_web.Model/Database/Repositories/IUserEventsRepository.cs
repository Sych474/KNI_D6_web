using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KNI_D6_web.Model.Database.Repositories
{
    public interface IUserEventsRepository
    {
        Task<UserEvent> FindUserEventAsync(string userId, int eventId);

        Task RemoveUserEventAsync(string userId, int eventId);

        Task AddUserEventAsync(UserEvent entity);
    }
}
