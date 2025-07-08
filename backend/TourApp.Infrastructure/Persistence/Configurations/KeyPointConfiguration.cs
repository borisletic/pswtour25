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
    public class KeyPointConfiguration : IEntityTypeConfiguration<KeyPoint>
    {
        public void Configure(EntityTypeBuilder<KeyPoint> builder)
        {
            builder.ToTable("KeyPoints");

            builder.HasKey(k => k.Id);

            builder.Property(k => k.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(k => k.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(k => k.ImageUrl)
                .HasMaxLength(500);

            builder.Property(k => k.Order)
                .IsRequired();

            // Configure Location value object
            builder.OwnsOne(k => k.Location, location =>
            {
                location.Property(l => l.Latitude)
                    .HasColumnName("Latitude")
                    .IsRequired();

                location.Property(l => l.Longitude)
                    .HasColumnName("Longitude")
                    .IsRequired();
            });

            builder.HasOne<Tour>()
                .WithMany(t => t.KeyPoints)
                .HasForeignKey(k => k.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
