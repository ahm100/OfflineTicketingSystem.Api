using Microsoft.EntityFrameworkCore;
using OfflineTicketingSystem.Api.Models;

namespace OfflineTicketingSystem.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Admin User",
                Email = "admin@example.com",
                Role = UserRole.Admin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };

            var employee = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Employee User",
                Email = "employee@example.com",
                Role = UserRole.Employee,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Employee123!")
            };

            await db.Users.AddRangeAsync(admin, employee);
            await db.SaveChangesAsync();
        }
    }
}
