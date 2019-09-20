using KNI_D6_web.Model;
using System.Collections.Generic;

namespace KNI_D6_web.ViewModels.Events
{
    public class EventsViewModel
    {
        public IEnumerable<Event> PastEvents { get; set; }

        public IEnumerable<Event> FutureEvents { get; set; }
    }
}
