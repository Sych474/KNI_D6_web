using System.Collections;
using System.Collections.Generic;

namespace KNI_D6_web.Model.Achievements
{
    public class AchievementsGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Achievement> Ahievements { get; set; }
    }
}
