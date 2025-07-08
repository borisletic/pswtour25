using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;

namespace TourApp.Infrastructure.Persistence.Configurations
{
    public class TouristConfiguration : IEntityTypeConfiguration<Tourist>
    {
        public void Configure(EntityTypeBuilder<Tourist> builder)
        {
            builder.Property(t => t.BonusPoints)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(t => t.InvalidProblemsCount)
                .HasDefaultValue(0);

            // Configure interests as a JSON column
            builder.Property(t => t.Interests)
                .HasConversion(
                    v => string.Join(',', v.Select(i => (int)i)),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(i => (Interest)int.Parse(i))
                          .ToList()
                )
                .HasMaxLength(100);

            // Configure relationships
            builder.HasMany<Purchase>()
                .WithOne()
                .HasForeignKey(p => p.TouristId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<Problem>()
                .WithOne()
                .HasForeignKey(p => p.TouristId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
