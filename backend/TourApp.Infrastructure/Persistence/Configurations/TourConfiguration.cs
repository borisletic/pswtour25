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
    public class TourConfiguration : IEntityTypeConfiguration<Tour>
    {
        public void Configure(EntityTypeBuilder<Tour> builder)
        {
            builder.ToTable("Tours");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(t => t.Difficulty)
                .IsRequired();

            builder.Property(t => t.Category)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.ScheduledDate)
                .IsRequired();

            // Configure Money value object
            builder.OwnsOne(t => t.Price, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasPrecision(18, 2)
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("EUR");
            });

            // Configure relationships
            builder.HasMany(t => t.KeyPoints)
                .WithOne()
                .HasForeignKey(k => k.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Ratings)
                .WithOne()
                .HasForeignKey(r => r.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Guide>()
                .WithMany()
                .HasForeignKey(t => t.GuideId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
