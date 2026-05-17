using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleEcoms.Data;
using SimpleEcoms.DTOs;
using SimpleEcoms.Models;

namespace SimpleEcoms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // GET: api/categories
        // ব্যবহার: সব ক্যাটাগরির লিস্ট দেখাবে (শুধু নাম, আইডি এবং প্রোডাক্ট কাউন্ট)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = c.Products.Count  // এই ক্যাটাগরির কতটি প্রোডাক্ট আছে
                })
                .OrderBy(c => c.Name)  // নাম অনুযায়ী সাজানো
                .ToListAsync();
            
            return Ok(new 
            { 
                success = true, 
                message = $"{categories.Count} categories found",
                data = categories 
            });
        }
        
        // GET: api/categories/5
        // ব্যবহার: একটি নির্দিষ্ট ক্যাটাগরির সম্পূর্ণ তথ্য
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (category == null)
            {
                return NotFound(new { success = false, message = $"Category with ID {id} not found" });
            }
            
            var categoryDetail = new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Products = category.Products.Select(p => new BasicProductInfoDto
                {
                    Id = p.Id,
                    Name = p.Name,
                }).ToList()
            };
            
            return Ok(new { success = true, data = categoryDetail });
        }
        
        // POST: api/categories
        // ব্যবহার: নতুন ক্যাটাগরি তৈরি (শুধু Admin)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto createDto)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, errors = ModelState });
            }
            
            // Check if category with same name already exists
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == createDto.Name.ToLower());
            
            if (existingCategory != null)
            {
                return Conflict(new { success = false, message = $"Category '{createDto.Name}' already exists" });
            }
            
            var category = new Category
            {
                Name = createDto.Name,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            
            // Return the created category
            var result = new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = 0,
                CreatedAt = category.CreatedAt,
                Products = new List<BasicProductInfoDto>()
            };
            
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, 
                new { success = true, message = "Category created successfully", data = result });
        }
        
        // PUT: api/categories/5
        // ব্যবহার: ক্যাটাগরি আপডেট (শুধু Admin)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, errors = ModelState });
            }
            
            var category = await _context.Categories.FindAsync(id);
            
            if (category == null)
            {
                return NotFound(new { success = false, message = $"Category with ID {id} not found" });
            }
            
            // Check if another category has the same name
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == updateDto.Name.ToLower() && c.Id != id);
            
            if (existingCategory != null)
            {
                return Conflict(new { success = false, message = $"Category '{updateDto.Name}' already exists" });
            }
            
            // Update properties
            category.Name = updateDto.Name;
            category.Description = updateDto.Description;
            category.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            // Return updated category
            var updatedCategory = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            var result = new CategoryDetailDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = updatedCategory?.Products.Count ?? 0,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Products = updatedCategory?.Products.Select(p => new BasicProductInfoDto
                {
                    Id = p.Id,
                    Name = p.Name,                    
                }).ToList() ?? new List<BasicProductInfoDto>()
            };
            
            return Ok(new { success = true, message = "Category updated successfully", data = result });
        }
        
        // DELETE: api/categories/5
        // ব্যবহার: ক্যাটাগরি ডিলিট (শুধু Admin)
        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (category == null)
            {
                return NotFound(new { success = false, message = $"Category with ID {id} not found" });
            }
            
            // Check if category has products
            if (category.Products.Any())
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = $"Cannot delete category '{category.Name}' because it has {category.Products.Count} products. Please reassign or delete the products first." 
                });
            }
            
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            
            return Ok(new { success = true, message = $"Category '{category.Name}' deleted successfully" });
        }
    }
}