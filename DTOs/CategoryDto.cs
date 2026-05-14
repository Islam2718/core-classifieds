using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SimpleEcoms.DTOs
{
    // লিস্ট view এর জন্য - শুধু মৌলিক তথ্য
    public class CategoryListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }  // এই ক্যাটাগরিতে কতটি প্রোডাক্ট আছে
    }
    
    // ডিটেইলস view এর জন্য - সম্পূর্ণ তথ্য
    public class CategoryDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public DateTime CreatedAt { get; set; }  // কখন তৈরি হয়েছে
        public DateTime? UpdatedAt { get; set; } // কখন শেষ আপডেট হয়েছে
        public List<BasicProductInfoDto> Products { get; set; } = new List<BasicProductInfoDto>();
    }
    
    // ক্যাটাগরির আওতাধীন প্রোডাক্টের মৌলিক তথ্য
    public class BasicProductInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
    
    // নতুন ক্যাটাগরি তৈরি করার সময় client কি পাঠাবে
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }
    
    // ক্যাটাগরি আপডেট করার সময় client কি পাঠাবে
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }
}