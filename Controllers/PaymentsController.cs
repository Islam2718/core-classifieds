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
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
        
        // POST: api/payments
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentCreateDto paymentDto)
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == paymentDto.OrderId && o.UserId == userId);
                
            if (order == null)
                return NotFound("Order not found");
                
            if (order.Status != "Pending")
                return BadRequest("Payment can only be processed for pending orders");
                
            var payment = new Payment
            {
                OrderId = paymentDto.OrderId,
                Amount = order.TotalAmount,
                PaymentMethod = paymentDto.PaymentMethod,
                TransactionId = paymentDto.TransactionId,
                PaymentStatus = "Completed"
            };
            
            _context.Payments.Add(payment);
            order.Status = "Processing";
            
            await _context.SaveChangesAsync();
            
            var paymentResponse = new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate
            };
            
            return Ok(paymentResponse);
        }
        
        // GET: api/payments/order/5
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var userId = GetUserId();
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
                
            if (order == null)
                return NotFound("Order not found");
                
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
                
            if (payment == null)
                return NotFound("Payment not found for this order");
                
            var paymentResponse = new PaymentResponseDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                PaymentDate = payment.PaymentDate
            };
            
            return Ok(paymentResponse);
        }
    }
}