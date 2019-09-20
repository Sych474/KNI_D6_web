using System;
using System.Collections.Generic;

namespace KNI_D6_web.Model
{
    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public bool IsSpecial { get; set; } = false;

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

        public int? SemesterId { get; set; } = null;

        public Semester Semester { get; set; }
    }
}
