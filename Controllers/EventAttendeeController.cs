using Microsoft.AspNetCore.Mvc;
using EventManagement.Models;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventAttendeeController : ControllerBase
    {
        public readonly AppDbContext _context;

        public EventAttendeeController(AppDbContext context)
        {
            _context = context;
        }

        // DTO for the import attendees request
        public class ImportAttendeeRequest
        {
            public int EventId { get; set; }
            public List<int> UserIds { get; set; }
        }

        // Response model for the import attendees response
        public class ImportAttendeesResponse
        {
            public bool Success { get; set; }
            public List<string> ConflictingAttendees { get; set; }
        }

        // API to import attendees and check for conflicts
        [HttpPost("import")]
        public async Task<ActionResult<ImportAttendeesResponse>> ImportAttendees([FromBody] ImportAttendeeRequest request)
        {
            // Validate input
            if (request.EventId <= 0)
            {
                return BadRequest("Invalid EventId.");
            }

            if (request.UserIds == null || !request.UserIds.Any())
            {
                return BadRequest("UserIds cannot be empty.");
            }

            // Fetch event details and existing attendees in one go
            var eventDetails = await _context.Events
                .Where(e => e.EventId == request.EventId)
                .Select(e => new { e.EventDateTime, e.EventId })
                .FirstOrDefaultAsync();

            if (eventDetails == null)
            {
                return NotFound("Event not found.");
            }

            // Prepare to check for conflicts
            var conflictingAttendees = new List<string>();
            var existingAttendees = await _context.EventAttendees
                .Where(ea => ea.EventId == request.EventId && request.UserIds.Contains(ea.UserId))
                .Select(ea => ea.UserId)
                .ToListAsync();

            foreach (var userId in request.UserIds)
            {
                // Check if user is already an attendee
                if (existingAttendees.Contains(userId))
                {
                    var conflictingUser = await _context.Users.FindAsync(userId);
                    if (conflictingUser != null)
                    {
                        conflictingAttendees.Add(conflictingUser.Name);
                    }
                    continue; // Skip adding this user as they're already an attendee
                }

                // Check for any conflicting event attendance
                var isConflicting = await _context.EventAttendees
                    .AnyAsync(ea => ea.UserId == userId && ea.Event.EventDateTime == eventDetails.EventDateTime);

                if (isConflicting)
                {
                    var conflictingUser = await _context.Users.FindAsync(userId);
                    if (conflictingUser != null)
                    {
                        conflictingAttendees.Add(conflictingUser.Name);
                    }
                }
                else
                {
                    // Add attendee if no conflict
                    var attendee = new EventAttendee { EventId = request.EventId, UserId = userId };
                    _context.EventAttendees.Add(attendee);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new ImportAttendeesResponse
            {
                Success = true,
                ConflictingAttendees = conflictingAttendees
            });
        }
    }
}
