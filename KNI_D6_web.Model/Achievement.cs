using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model
{
    public class Achievement
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<AchievementParameter> AchievementParameters { get; set; } = new List<AchievementParameter>();

        public List<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
        
        //TO_DO
    }
}
