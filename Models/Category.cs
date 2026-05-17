using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleEcoms.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public int? ParentId { get; set; } = null;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}