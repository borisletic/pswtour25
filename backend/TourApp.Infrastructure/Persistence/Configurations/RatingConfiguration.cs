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
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("Ratings");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Score)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasOne<Tour>()
                .WithMany(t => t.Ratings)
                .HasForeignKey(r => r.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Tourist>()
                .WithMany()
                .HasForeignKey(r => r.TouristId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure unique rating per tourist per tour
            builder.HasIndex(r => new { r.TouristId, r.TourId })
                .IsUnique();
        }
    }
}
