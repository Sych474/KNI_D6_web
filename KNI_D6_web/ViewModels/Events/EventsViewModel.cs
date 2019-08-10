using KNI_D6_web.Model;
using System.Collections.Generic;

namespace KNI_D6_web.ViewModels.Events
{
    public class EventsViewModel
    {
        public bool IsAdmin { get; set; }

        public IEnumerable<Event> Events { get; set; } 
    }
}
