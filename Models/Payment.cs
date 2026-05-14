using System.ComponentModel.DataAnnotations;

namespace SimpleEcoms.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // CreditCard, PayPal, BankTransfer
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string TransactionId { get; set; } = string.Empty;
        
        // Navigation property
        public Order? Order { get; set; }
    }
}