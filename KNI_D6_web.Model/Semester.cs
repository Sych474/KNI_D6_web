using KNI_D6_web.Model.Achievements;
using System.Collections.Generic;

namespace KNI_D6_web.Model
{
    public class Semester
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Achievement> Achievements { get; set; } = new List<Achievement>();

        public IEnumerable<Event> Events { get; set; } = new List<Event>();
    }
}
