using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Events;

namespace TourApp.Infrastructure.Persistence.Configurations
{
    public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            builder.ToTable("Problems");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            // Configure events as JSON column
            builder.Property(p => p.Events)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<ProblemEvent>>(v, (JsonSerializerOptions)null)
                )
                .HasColumnType("json");

            builder.HasOne<Tour>()
                .WithMany()
                .HasForeignKey(p => p.TourId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Tourist>()
                .WithMany()
                .HasForeignKey(p => p.TouristId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
