using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourApp.Application.Services;
using TourApp.Domain.Entities;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TourAppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            try
            {
                // Apply pending migrations
                await context.Database.MigrateAsync();

                // Seed data
                await SeedDataAsync(context, passwordHasher);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        private static async Task SeedDataAsync(TourAppDbContext context, IPasswordHasher passwordHasher)
        {
            // Seed Administrator
            if (!await context.Administrators.AnyAsync())
            {
                var admin = new Administrator(
                    "admin",
                    "admin@tourapp.com",
                    passwordHasher.HashPassword("Admin123!"),
                    "System",
                    "Administrator"
                );

                context.Administrators.Add(admin);
            }

            // Seed Guides
            if (!await context.Guides.AnyAsync())
            {
                var guides = new List<Guide>
                {
                    new Guide(
                        "john_guide",
                        "john.guide@tourapp.com",
                        passwordHasher.HashPassword("Guide123!"),
                        "John",
                        "Smith"
                    ),
                    new Guide(
                        "sarah_guide",
                        "sarah.guide@tourapp.com",
                        passwordHasher.HashPassword("Guide123!"),
                        "Sarah",
                        "Johnson"
                    ),
                    new Guide(
                        "mike_guide",
                        "mike.guide@tourapp.com",
                        passwordHasher.HashPassword("Guide123!"),
                        "Mike",
                        "Williams"
                    )
                };

                context.Guides.AddRange(guides);
            }

            await context.SaveChangesAsync();
        }
    }
}
