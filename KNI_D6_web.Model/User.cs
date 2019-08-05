using KNI_D6_web.Model.Parameters;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KNI_D6_web.Model
{
    public class User : IdentityUser
    {
        public List<ParameterValue> ParameterValues { get; set; } = new List<ParameterValue>();

        public List<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

        public List<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

        public List<NewsPost> NewsPosts { get; set; } = new List<NewsPost>(); 
    }
}
