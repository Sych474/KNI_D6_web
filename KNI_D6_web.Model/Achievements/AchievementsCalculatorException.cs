using System;
using System.Runtime.Serialization;

namespace KNI_D6_web.Model.Achievements
{
    class AchievementCalculatorException : Exception
    {
        public AchievementCalculatorException()
        {
        }

        public AchievementCalculatorException(string message) : base(message)
        {
        }

        public AchievementCalculatorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AchievementCalculatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
