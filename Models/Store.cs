using System.ComponentModel.DataAnnotations;

namespace SimpleEcoms.Models
{
    public class Store
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Thana { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string TaxIdentificationNumber { get; set; } = string.Empty;
        public string TradeLicenseNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}