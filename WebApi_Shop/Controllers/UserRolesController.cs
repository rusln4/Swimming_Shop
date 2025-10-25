using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Shop.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly SwimShopDbContext _context;

        public UserRolesController(SwimShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetUserRoles()
        {
            return await _context.UserRoles.ToListAsync();
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRole>> GetUserRole(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);

            if (userRole == null)
            {
                return NotFound();
            }

            return userRole;
        }

        [HttpPost]
        public async Task<ActionResult<UserRole>> PostUserRole(UserRole userRole)
        {
            // Проверяем, существует ли уже роль с таким названием
            if (await _context.UserRoles.AnyAsync(r => r.NameUserRole == userRole.NameUserRole))
            {
                return BadRequest("Роль с таким названием уже существует");
            }

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserRole", new { id = userRole.IdUserRoles }, userRole);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserRole(int id, UserRole userRole)
        {
            if (id != userRole.IdUserRoles)
            {
                return BadRequest();
            }

            // Проверяем, существует ли уже роль с таким названием (кроме текущей)
            if (await _context.UserRoles.AnyAsync(r => r.NameUserRole == userRole.NameUserRole && r.IdUserRoles != id))
            {
                return BadRequest("Роль с таким названием уже существует");
            }

            _context.Entry(userRole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserRoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }

            // Проверяем, используется ли роль пользователями
            if (await _context.Users.AnyAsync(u => u.RoleUser == id))
            {
                return BadRequest("Невозможно удалить роль, так как она используется пользователями");
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserRoleExists(int id)
        {
            return _context.UserRoles.Any(e => e.IdUserRoles == id);
        }
    }
}