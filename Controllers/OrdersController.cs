using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleEcoms.Data;
using SimpleEcoms.DTOs;
using SimpleEcoms.Models;
using System.Security.Claims;

namespace SimpleEcoms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
        
        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ShippingAddress = o.ShippingAddress,
                    Items = o.OrderItems.Select(oi => new OrderItemResponseDto
                    {
                        ProductName = oi.Product != null ? oi.Product.Name : "",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Quantity * oi.UnitPrice
                    }).ToList()
                })
                .ToListAsync();
                
            return Ok(orders);
        }
        
        // GET: api/orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .Where(o => o.Id == id && o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();
                
            if (order == null)
                return NotFound();
                
            var orderDto = new OrderResponseDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductName = oi.Product != null ? oi.Product.Name : "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Quantity * oi.UnitPrice
                }).ToList()
            };
                
            return Ok(orderDto);
        }
        
        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDto orderDto)
        {
            var userId = GetUserId();
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();
            
            foreach (var item in orderDto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Product with ID {item.ProductId} not found");
                    
                if (product.StockQuantity < item.Quantity)
                    return BadRequest($"Insufficient stock for product {product.Name}");
                    
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
                
                totalAmount += product.Price * item.Quantity;
                orderItems.Add(orderItem);
                
                // Update stock
                product.StockQuantity -= item.Quantity;
            }
            
            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                ShippingAddress = orderDto.ShippingAddress,
                Status = "Pending",
                OrderItems = orderItems
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        
        // PUT: api/orders/5/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
                
            if (order == null)
                return NotFound();
                
            if (order.Status != "Pending")
                return BadRequest("Only pending orders can be cancelled");
                
            order.Status = "Cancelled";
            
            // Restore stock
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == id)
                .ToListAsync();
                
            foreach (var item in orderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                    product.StockQuantity += item.Quantity;
            }
            
            await _context.SaveChangesAsync();
            return Ok(new { message = "Order cancelled successfully" });
        }

        // order and order items delete (admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
                
            if (order == null)
                return NotFound();
                
            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Order deleted successfully" });
        }
    }
}