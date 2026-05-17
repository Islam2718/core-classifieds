using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleEcoms.Data;
using SimpleEcoms.Models;
using SimpleEcoms.DTOs;

[Route("api/[controller]")]
[ApiController]
public class StoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ CREATE
    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Create(CreateStoreDto dto)
    {
        var userId = 1; // 🔥 later JWT থেকে নিবা

        var store = new Store
        {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Address = dto.Address,
            Division = dto.Division,
            District = dto.District,
            Thana = dto.Thana,
            ZipCode = dto.ZipCode,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Website = dto.Website,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            TradeLicenseNumber = dto.TradeLicenseNumber,
            UserId = userId
        };

        _context.Stores.Add(store);
        await _context.SaveChangesAsync();

        return Ok(store);
    }

    // ✅ GET ALL 
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stores = await _context.Stores
            .Select(s => new StoreResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Division = s.Division,
                District = s.District,
                PhoneNumber = s.PhoneNumber
            })
            .ToListAsync();

        return Ok(stores);
    }

    // ✅ GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var store = await _context.Stores.FindAsync(id);

        if (store == null)
            return NotFound();

        return Ok(store);
    }

    // ✅ UPDATE
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Update(int id, UpdateStoreDto dto)
    {
        var store = await _context.Stores.FindAsync(id);

        if (store == null)
            return NotFound();

        store.Name = dto.Name;
        store.Description = dto.Description;
        store.ImageUrl = dto.ImageUrl;
        store.Address = dto.Address;
        store.Division = dto.Division;
        store.District = dto.District;
        store.Thana = dto.Thana;
        store.ZipCode = dto.ZipCode;
        store.PhoneNumber = dto.PhoneNumber;
        store.Email = dto.Email;
        store.Website = dto.Website;
        store.TaxIdentificationNumber = dto.TaxIdentificationNumber;
        store.TradeLicenseNumber = dto.TradeLicenseNumber;
        store.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(store);
    }

    // ✅ DELETE
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Delete(int id)
    {
        var store = await _context.Stores.FindAsync(id);

        if (store == null)
            return NotFound();

        _context.Stores.Remove(store);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Store deleted successfully" });
    }
}