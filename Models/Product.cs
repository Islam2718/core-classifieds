using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleEcoms.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        
        // Foreign Key
        public int CategoryId { get; set; }
        
        [JsonIgnore]
        public Category? Category { get; set; }
        
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}