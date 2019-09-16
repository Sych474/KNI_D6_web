using System.Collections.Generic;

namespace KNI_D6_web.ViewModels.Events
{
    public class EventVisitorsViewModel
    {
        public int EventId { get; set; }

        public IEnumerable<EventVisitorViewModel> EventVisitors { get; set; }
    }
}
