namespace KNI_D6_web.Model.Parameters
{
    public class ParameterValue
    {
        public const int DefaultValue = 0;

        public int ParameterId { get; set; }

        public Parameter Parameter { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public int Value { get; set; } = DefaultValue;
    }
}
