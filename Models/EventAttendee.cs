namespace EventManagement.Models
{
    public class EventAttendee
    {
        public int EventAttendeeId { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        
        public int UserId { get; set; }
        
        public User User { get; set; }
    }
}
