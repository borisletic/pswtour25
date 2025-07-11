using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourApp.Domain.Entities;
using TourApp.Infrastructure.Persistence.Configurations;

namespace TourApp.Infrastructure.Persistence.Context
{
    public class TourAppDbContext : DbContext
    {
        public DbSet<Tourist> Tourists { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<KeyPoint> KeyPoints { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<MonthlyReport> MonthlyReports { get; set; }

        public TourAppDbContext(DbContextOptions<TourAppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ignore the Event classes - they are only for JSON serialization, not EF entities
            modelBuilder.Ignore<TourApp.Domain.Events.ProblemEvent>();
            modelBuilder.Ignore<TourApp.Domain.Events.ProblemCreatedEvent>();
            modelBuilder.Ignore<TourApp.Domain.Events.ProblemStatusChangedEvent>();
            modelBuilder.Ignore<TourApp.Domain.Events.TourCancelledEvent>();

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TouristConfiguration());
            modelBuilder.ApplyConfiguration(new GuideConfiguration());
            modelBuilder.ApplyConfiguration(new TourConfiguration());
            modelBuilder.ApplyConfiguration(new KeyPointConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseConfiguration());
            modelBuilder.ApplyConfiguration(new RatingConfiguration());
            modelBuilder.ApplyConfiguration(new ProblemConfiguration());
            modelBuilder.ApplyConfiguration(new MonthlyReportConfiguration());
        }
    }
}
