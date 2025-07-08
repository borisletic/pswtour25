using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;

namespace TourApp.Infrastructure.Persistence.Configurations
{
    public class GuideConfiguration : IEntityTypeConfiguration<Guide>
    {
        public void Configure(EntityTypeBuilder<Guide> builder)
        {
            builder.Property(g => g.CancelledToursCount)
                .HasDefaultValue(0);

            builder.Property(g => g.RewardPoints)
                .HasDefaultValue(0);

            // Configure relationships
            builder.HasMany<Tour>()
                .WithOne()
                .HasForeignKey(t => t.GuideId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
