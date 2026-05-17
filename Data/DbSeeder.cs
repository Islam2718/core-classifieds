using Microsoft.Extensions.DependencyInjection;
using SimpleEcoms.Models;
using SimpleEcoms.Data;
using BCrypt.Net;

namespace SimpleEcoms.Data
{
    public static class DbSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            if (!context.Users.Any())
            {
                // User create 
                context.Users.Add(new User
                {
                    Username = "SuperAdmin",
                    Email = "admin@mail.com",
                    Phone = "01710001337",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                });

                context.SaveChanges();

                // Store create 
                var store = new Store
                {
                    Name = "Krishikhamar",
                    Description = "Krishikhamar",
                    Address = "Dhaka",
                    PhoneNumber = "01700000000",
                    Email = "store@site.com",
                    Division = "Dhaka",
                    District = "Dhaka",
                    Thana = "Mohammadpur",
                    ZipCode = "1207",
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow
                };

                context.Stores.Add(store);
                context.SaveChanges();                
            }
        }
    }
}