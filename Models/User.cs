namespace EventManagement.Models
{
 public class User
{
    public int UserId { get; set; }

    public required string Name { get; set; } // Make Name required
    public ICollection<EventAttendee> EventAttendees { get; set; } = new List<EventAttendee>(); // Initialize to avoid null
}

}
