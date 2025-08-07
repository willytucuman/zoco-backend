using BackendZocoUsers.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendZocoUsers.Data
{
    public static class DbSeeder
    {
        public static async Task RunAsync(AppDbContext ctx)
        {
            await ctx.Database.MigrateAsync();

            if (!ctx.Users.Any())
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = "admin@zoco.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = Role.Admin
                };
                var user = new User
                {
                    Name = "Usuario Demo",
                    Email = "user@zoco.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    Role = Role.User
                };

                ctx.Users.AddRange(admin, user);
                await ctx.SaveChangesAsync();

                ctx.Studies.AddRange(
                    new Study { Title = "Ing. Sistemas", Institution = "UNT", UserId = user.Id },
                    new Study { Title = "Arquitectura", Institution = "UBA", UserId = user.Id }
                );

                ctx.Addresses.AddRange(
                    new Address { Street = "Muñecas 395", City = "San Miguel de Tucumán", UserId = user.Id },
                    new Address { Street = "Salta 381", City = "San Miguel de Tucumán", UserId = user.Id }
                );

                await ctx.SaveChangesAsync();
            }
        }
    }
}
