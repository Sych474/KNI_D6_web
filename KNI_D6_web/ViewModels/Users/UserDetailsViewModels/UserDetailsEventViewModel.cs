using System;

namespace KNI_D6_web.ViewModels.Users.UserDetailsViewModels
{
    public class UserDetailsEventViewModel
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        public DateTime Date { get; set; }

        public EventVisitState State { get; set; }
    }
}
