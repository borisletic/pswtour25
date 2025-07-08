using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Infrastructure.Persistence.Context;
using TourApp.Infrastructure.Persistence.Seed;

namespace TourApp.Infrastructure.Migrations
{
    public static class DbInitializer
    {
        public static async Task InitializeDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<TourAppDbContext>();
            var seeder = serviceScope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed data
            await seeder.SeedAsync();
        }
    }
}
