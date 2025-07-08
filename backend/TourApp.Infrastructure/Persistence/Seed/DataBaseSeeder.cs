using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Services;
using TourApp.Domain.Entities;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.Infrastructure.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private readonly TourAppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseSeeder(TourAppDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            if (await _context.Database.CanConnectAsync())
            {
                await SeedAdministratorsAsync();
                await SeedGuidesAsync();
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedAdministratorsAsync()
        {
            if (!await _context.Administrators.AnyAsync())
            {
                var admin = new Administrator(
                    "admin",
                    "admin@touristtours.com",
                    _passwordHasher.HashPassword("Admin123!"),
                    "System",
                    "Administrator"
                );

                await _context.Administrators.AddAsync(admin);
            }
        }

        private async Task SeedGuidesAsync()
        {
            if (!await _context.Guides.AnyAsync())
            {
                var guides = new List<Guide>
                {
                    new Guide(
                        "john_guide",
                        "john.guide@touristtours.com",
                        _passwordHasher.HashPassword("Guide123!"),
                        "John",
                        "Smith"
                    ),
                    new Guide(
                        "sarah_guide",
                        "sarah.guide@touristtours.com",
                        _passwordHasher.HashPassword("Guide123!"),
                        "Sarah",
                        "Johnson"
                    ),
                    new Guide(
                        "mike_guide",
                        "mike.guide@touristtours.com",
                        _passwordHasher.HashPassword("Guide123!"),
                        "Mike",
                        "Williams"
                    )
                };

                await _context.Guides.AddRangeAsync(guides);
            }
        }
    }
}
