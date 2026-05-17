using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleEcoms.Data;
using SimpleEcoms.DTOs;
using SimpleEcoms.Models;
using SimpleEcoms.Services;
using BCrypt.Net;
using System.Security.Claims;

namespace SimpleEcoms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // Check if user exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "User already exists" });
            }

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                Phone = registerDto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = "User",
                //additional fields 
                Address = registerDto.Address,
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender,
                Image = registerDto.Image
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role
                }
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Stores)
                .FirstOrDefaultAsync(u => 
                    u.Email == loginDto.Identifier || 
                    u.Phone == loginDto.Identifier
                );

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    StoreIds = user.Stores.Select(s => s.Id).ToList()
                }
            });
        }

        // logout method 
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }
        // [HttpPost("logout")]
        // [Authorize]
        // public async Task<IActionResult> Logout()
        // {
        //     var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        //     if (userIdClaim == null)
        //         return Unauthorized();

        //     var userId = int.Parse(userIdClaim.Value);

        //     var refreshToken = await _context.RefreshTokens
        //         .FirstOrDefaultAsync(x => x.UserId == userId);

        //     if (refreshToken != null)
        //     {
        //         _context.RefreshTokens.Remove(refreshToken);
        //         await _context.SaveChangesAsync();
        //     }

        //     return Ok(new { message = "Logged out successfully" });
        // }
    }
}