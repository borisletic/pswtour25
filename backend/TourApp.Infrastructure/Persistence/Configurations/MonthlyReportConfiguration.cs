using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TourApp.Domain.Entities;

namespace TourApp.Infrastructure.Persistence.Configurations
{
    public class MonthlyReportConfiguration : IEntityTypeConfiguration<MonthlyReport>
    {
        public void Configure(EntityTypeBuilder<MonthlyReport> builder)
        {
            builder.ToTable("MonthlyReports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Month)
                .IsRequired();

            builder.Property(r => r.Year)
                .IsRequired();

            builder.Property(r => r.GeneratedAt)
                .IsRequired();

            // Configure TourSales as JSON
            builder.Property(r => r.TourSales)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<TourSalesInfo>>(v, (JsonSerializerOptions)null)
                )
                .HasColumnType("json");

            builder.HasOne<Guide>()
                .WithMany()
                .HasForeignKey(r => r.GuideId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure unique report per guide per month
            builder.HasIndex(r => new { r.GuideId, r.Month, r.Year })
                .IsUnique();
        }
    }
}
