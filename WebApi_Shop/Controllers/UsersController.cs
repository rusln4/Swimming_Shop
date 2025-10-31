using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Shop.Models;
using WebApi_Shop.Models.Requests;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SwimShopDbContext _context;

        public UsersController(SwimShopDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers() => await _context.Users.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            if (request is null)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(request.MailUser) || string.IsNullOrWhiteSpace(request.PasswordUser))
            {
                return BadRequest("Нет данных");
            }

            if (await _context.Users.AnyAsync(u => u.MailUser == request.MailUser))
            {
                return BadRequest("Почта уже используется");
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

            return CreatedAtAction(nameof(GetUser), new { id = user.IdUsers }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request is null)
            {
                return BadRequest();
            } 
            if (string.IsNullOrWhiteSpace(request.MailUser) || string.IsNullOrWhiteSpace(request.PasswordUser))
            {
                return BadRequest("Пустые поля");
            }
                

            var user = await _context.Users.FirstOrDefaultAsync(u => u.MailUser == request.MailUser && u.PasswordUser == request.PasswordUser);

            if (user is null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                user.IdUsers,
                user.MailUser,
                user.NameUser,
                user.LastnameUser,
                user.PhoneUser,
                user.AddressUser,
                user.RoleUser
            });
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (request is null)
            {
                return BadRequest();
            }
            if (request.IdUsers.HasValue && request.IdUsers.Value != id)
            {
                return BadRequest();
            }
                

            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound();
            } 

            if (!string.IsNullOrWhiteSpace(request.NameUser))
            {
                user.NameUser = request.NameUser;
            } 
            if (!string.IsNullOrWhiteSpace(request.LastnameUser))
            {
                user.LastnameUser = request.LastnameUser;
            } 
            if (!string.IsNullOrWhiteSpace(request.AddressUser))
            {
                user.AddressUser = request.AddressUser;
            }
            if (!string.IsNullOrWhiteSpace(request.PasswordUser))
            {
                user.PasswordUser = request.PasswordUser;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.IdUsers == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("usesrs/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return NotFound();
            } 

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        
    }
}
