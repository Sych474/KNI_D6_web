using System.Collections.Generic;

namespace KNI_D6_web.Model.Parameters
{
    public class Parameter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ParameterType ParameterType { get; set; }

        public List<ParameterValue> ParameterValues { get; set; } = new List<ParameterValue>();

        public List<AchievementParameter> AchievementParameters { get; set; } = new List<AchievementParameter>();
    }
}
