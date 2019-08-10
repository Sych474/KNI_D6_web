using KNI_D6_web.Model;
using System;

namespace KNI_D6_web.ViewModels.Users
{
    public class UserDetailsEventViewModel
    {
        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public bool IsVisited { get; set; }

        public UserDetailsEventViewModel(Event @event, bool isVisited)
        {
            EventName = @event.Name;
            EventDate = @event.Date;
            IsVisited = isVisited;
        }

        public UserDetailsEventViewModel()
        {
        }
    }
}
