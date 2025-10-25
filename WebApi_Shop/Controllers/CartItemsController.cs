using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Shop.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly SwimShopDbContext _context;

        public CartItemsController(SwimShopDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            return await _context.CartItems.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return cartItem;
        }

       
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItemsByUser(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.IdUserCart == userId)
                .Include(c => c.IdProductCartNavigation)
                .ToListAsync();

            return cartItems;
        }

        
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItem cartItem)
        {
            // Проверяем, существует ли уже такой товар в корзине пользователя
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.IdUserCart == cartItem.IdUserCart && 
                                         c.IdProductCart == cartItem.IdProductCart);

            if (existingItem != null)
            {
                // Если товар уже есть, увеличиваем количество
                existingItem.Quantity += cartItem.Quantity;
                existingItem.AddedDate = DateTime.Now;
                
                _context.Entry(existingItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                return CreatedAtAction("GetCartItem", new { id = existingItem.IdCartItem }, existingItem);
            }
            
            // Если товара нет, добавляем новый
            cartItem.AddedDate = DateTime.Now;
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartItem", new { id = cartItem.IdCartItem }, cartItem);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(int id, CartItem cartItem)
        {
            if (id != cartItem.IdCartItem)
            {
                return BadRequest();
            }

            _context.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
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
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> ClearUserCart(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.IdUserCart == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return NotFound("Корзина пользователя пуста");
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.IdCartItem == id);
        }
    }
}