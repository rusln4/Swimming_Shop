using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Shop.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SwimShopDbContext _context;

        public UsersController(SwimShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public Task<ActionResult<User>> GetUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.IdUsers == id);
            if (user == null)
            {
                return Task.FromResult<ActionResult<User>>(NotFound());  
            }
            return Task.FromResult<ActionResult<User>>(Ok(user));
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            if (_context.Users.Any(u => u.MailUser == request.MailUser))
            {
                return BadRequest("This email is already registered");
            }

            var user = new User
            {
                RoleUser = 2,
                MailUser = request.MailUser,
                PasswordUser = request.PasswordUser,
                NameUser = request.NameUser,
                LastnameUser = request.LastnameUser,
                PhoneUser = request.PhoneUser,
                AddressUser = request.AddressUser
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.IdUsers }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, object> body)
        {
            if (body == null)
                return BadRequest("Body is required.");
        
            // Make key lookup case-insensitive (MailUser/mailUser etc.)
            var dict = new Dictionary<string, object>(body, System.StringComparer.OrdinalIgnoreCase);
        
            if (!dict.TryGetValue("MailUser", out var mailObj) || !dict.TryGetValue("PasswordUser", out var passObj))
                return BadRequest("MailUser and PasswordUser are required.");
        
            var mail = mailObj?.ToString();
            var password = passObj?.ToString();
        
            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
                return BadRequest("MailUser and PasswordUser must be non-empty.");
        
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.MailUser == mail && u.PasswordUser == password);
        
            if (user == null)
                return Unauthorized();
        
            return Ok(new
            {
                IdUsers = user.IdUsers,
                MailUser = user.MailUser,
                NameUser = user.NameUser,
                LastnameUser = user.LastnameUser,
                PhoneUser = user.PhoneUser,
                AddressUser = user.AddressUser,
                RoleUser = user.RoleUser
            });
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(u => u.IdUsers == id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] Dictionary<string, object> body)
        {
            if (body == null)
                return BadRequest("Body is required.");
        
            // Case-insensitive key lookup for IdUsers/nameUser/etc.
            var dict = new Dictionary<string, object>(body, System.StringComparer.OrdinalIgnoreCase);
        
            if (dict.TryGetValue("IdUsers", out var idObj)
                && int.TryParse(idObj?.ToString(), out var idFromBody)
                && idFromBody != id)
            {
                return BadRequest("ID in body does not match route.");
            }
        
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
        
            if (dict.TryGetValue("NameUser", out var nameObj) && nameObj is not null)
                user.NameUser = nameObj.ToString();
        
            if (dict.TryGetValue("LastnameUser", out var lastObj) && lastObj is not null)
                user.LastnameUser = lastObj.ToString();
        
            if (dict.TryGetValue("AddressUser", out var addrObj) && addrObj is not null)
                user.AddressUser = addrObj.ToString();
        
            if (dict.TryGetValue("PasswordUser", out var passObj))
            {
                var pass = passObj?.ToString();
                if (!string.IsNullOrWhiteSpace(pass))
                    user.PasswordUser = pass;
            }
        
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.IdUsers == id))
                    return NotFound();
                throw;
            }
        
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class RegisterRequest
    {
        public string MailUser { get; set; } = null!;
        public string PasswordUser { get; set; } = null!;
        public string NameUser { get; set; } = null!;
        public string LastnameUser { get; set; } = null!;
        public string PhoneUser { get; set; } = null!;
        public string AddressUser { get; set; } = null!;
    }

    public class LoginRequest
    {
        public string MailUser { get; set; } = null!;
        public string PasswordUser { get; set; } = null!;
    }
}
