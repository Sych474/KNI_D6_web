using System;

namespace KNI_D6_web.ViewModels.Visits
{
    public class EventVisitViewModel
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public DateTime Date { get; set; }

        public EventVisitState State { get; set; }
    }
}
