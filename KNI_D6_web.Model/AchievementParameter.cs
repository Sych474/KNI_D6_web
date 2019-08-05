using KNI_D6_web.Model.Parameters;

namespace KNI_D6_web.Model
{
    public class AchievementParameter
    {
        public int AchievementId { get; set; }

        public Achievement Achievement { get; set; }

        public int ParameterId { get; set; }

        public Parameter Parameter { get; set; }
    }
}
