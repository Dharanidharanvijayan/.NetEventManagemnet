namespace EventManagement.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public DateTime EventDateTime { get; set; }
        public ICollection<EventAttendee> EventAttendees { get; set; }
    }
}
