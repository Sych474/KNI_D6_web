namespace KNI_D6_web.Model
{
    public class UserEvent
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
