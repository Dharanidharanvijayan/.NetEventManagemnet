using Microsoft.AspNetCore.Mvc;
using EventManagement.Models;
using EventManagement.Data;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

 [HttpPost]
public async Task<ActionResult<User>> AddUser(User user)
{
    // Validate the incoming user data
    if (user == null)
    {
        return BadRequest("User cannot be null.");
    }

    if (string.IsNullOrWhiteSpace(user.Name))
    {
        return BadRequest("Name is required.");
    }

    // Check for existing user with the same UserId
    var existingUser = await _context.Users.FindAsync(user.UserId);
    if (existingUser != null)
    {
        return Conflict("A user with the same UserId already exists.");
    }

    // Add the user to the context
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();

    return Ok(user);
}

}
