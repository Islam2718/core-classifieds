using System.ComponentModel.DataAnnotations;

namespace SimpleEcoms.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        public string ShippingAddress { get; set; } = string.Empty;
        
        // Navigation properties
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }
}