using KNI_D6_web.Model.Achievements;
using System.Collections.Generic;

namespace KNI_D6_web.Model.Parameters
{
    public class Parameter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ParameterValue> ParameterValues { get; set; } = new List<ParameterValue>();

        public List<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
