using System.Collections.Generic;

namespace KNI_D6_web.Model.Database.Initialization.Configuration
{
    public class DbInitializationConfiguration
    {
        public IEnumerable<string> Parameters { get; set; }

        public IEnumerable<string> Achievements { get; set; }

        public IEnumerable<ParameterValuesPreset> ParameterValuesPresets { get; set; }

        public IEnumerable<UserAchievementsPreset> UserAchievementsPresets { get; set; }

        public string AdminLogin { get; set; }

        public string AdminPassword { get; set; }

        public string AdminEmail { get; set; }
    }
}
