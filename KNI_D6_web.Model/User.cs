using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace KNI_D6_web.Model
{
    public class User : IdentityUser
    {
        public bool IsKniAdmin { get; set; } = false;

        public UserPosition Position { get; set; }

        public List<ParameterValue> ParameterValues { get; set; } = new List<ParameterValue>();

        public List<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

        public List<NewsPost> NewsPosts { get; set; } = new List<NewsPost>(); 
    }
}
