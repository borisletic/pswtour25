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
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");

            builder.HasKey(p => p.Id);

            builder.OwnsOne(p => p.TotalPrice, price =>
            {
                price.Property(pr => pr.Amount)
                    .HasColumnName("TotalPrice")
                    .HasPrecision(18, 2)
                    .IsRequired();

                price.Property(pr => pr.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("EUR");
            });

            builder.Property(p => p.BonusPointsUsed)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            builder.Property(p => p.PurchasedAt)
                .IsRequired();

            builder.HasOne<Tourist>()
                .WithMany()
                .HasForeignKey(p => p.TouristId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<Tour>()
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "PurchaseTours",
                    j => j.HasOne<Tour>().WithMany().HasForeignKey("TourId"),
                    j => j.HasOne<Purchase>().WithMany().HasForeignKey("PurchaseId")
                );
        }
    }
}
