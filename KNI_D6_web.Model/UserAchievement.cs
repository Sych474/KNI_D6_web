using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model
{
    public class UserAchievement
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public int AchievementId { get; set; }

        public Achievement Achievement { get; set; }
    }
}
