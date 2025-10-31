using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi_Shop.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly SwimShopDbContext _context;

        public OrderItemsController(SwimShopDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        {
            return await _context.OrderItems.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return orderItem;
        }

        
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItemsByOrder(int orderId)
        {
            var orderItems = await _context.OrderItems.Where(o => o.IdOrder == orderId).Include(o => o.IdProductNavigation).ToListAsync();

            if (!orderItems.Any())
            {
                return NotFound("Элементы заказа не найдены");
            }

            return orderItems;
        }

       
        [HttpPost]
        public async Task<ActionResult<OrderItem>> PostOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderItem", new { id = orderItem.IdOrderItem }, orderItem);
        }

        
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> PostOrderItems(List<OrderItem> orderItems)
        {
            if (orderItems == null || !orderItems.Any())
            {
                return BadRequest("Список элементов заказа пуст");
            }

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderItemsByOrder", new { orderId = orderItems.First().IdOrder }, orderItems);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderItem(int id, OrderItem orderItem)
        {
            if (id != orderItem.IdOrderItem)
            {
                return BadRequest();
            }

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
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
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.IdOrderItem == id);
        }
    }
}