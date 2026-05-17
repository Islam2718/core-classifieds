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

        public string ImageUrl { get; set; } = string.Empty;

        // Foreign Key
        public int CategoryId { get; set; }        
        [JsonIgnore]
        public Category? Category { get; set; }

        public int StoreId { get; set; }
        [JsonIgnore]
        public Store? Store { get; set; }
    }
}