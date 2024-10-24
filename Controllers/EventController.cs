using Microsoft.AspNetCore.Mvc;
using EventManagement.Models;
using EventManagement.Data;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly AppDbContext _context;

    public EventController(AppDbContext context)
    {
        _context = context;
    }

    // API to create an event
    [HttpPost]
    public async Task<ActionResult<Event>> CreateEvent(Event newEvent)
    {
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        return Ok(newEvent);
    }

    // API to get all events
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
    {
        return Ok(await _context.Events.ToListAsync());
    }

    // API to get upcoming events
    [HttpGet("upcoming")]
    public async Task<ActionResult<IEnumerable<Event>>> GetUpcomingEvents()
    {
        var upcomingEvents = await _context.Events
            .Where(e => e.EventDateTime > DateTime.Now)
            .ToListAsync();
        return Ok(upcomingEvents);
    }

    // API to get past events
    [HttpGet("past")]
    public async Task<ActionResult<IEnumerable<Event>>> GetPastEvents()
    {
        var pastEvents = await _context.Events
            .Where(e => e.EventDateTime < DateTime.Now)
            .ToListAsync();
        return Ok(pastEvents);
    }
}
