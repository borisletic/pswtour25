using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.ValueObjects;

namespace TourApp.Domain.Entities
{
    public class Purchase
    {
        private readonly List<Tour> _tours = new List<Tour>();

        public Guid Id { get; private set; }
        public Guid TouristId { get; private set; }
        public IReadOnlyCollection<Tour> Tours => _tours.AsReadOnly();
        public Money TotalPrice { get; private set; }
        public decimal BonusPointsUsed { get; private set; }
        public DateTime PurchasedAt { get; private set; }

        protected Purchase() { }

        public Purchase(Guid touristId, List<Tour> tours, Money totalPrice, decimal bonusPointsUsed = 0)
        {
            if (tours == null || !tours.Any())
                throw new ArgumentException("Purchase must contain at least one tour");

            Id = Guid.NewGuid();
            TouristId = touristId;
            _tours.AddRange(tours);
            TotalPrice = totalPrice ?? throw new ArgumentNullException(nameof(totalPrice));
            BonusPointsUsed = bonusPointsUsed;
            PurchasedAt = DateTime.UtcNow;

            ValidatePurchase();
        }

        private void ValidatePurchase()
        {
            // Ensure all tours are published
            if (_tours.Any(t => t.Status != TourStatus.Published))
                throw new InvalidOperationException("Cannot purchase unpublished tours");

            // Ensure all tours are in the future
            if (_tours.Any(t => t.ScheduledDate <= DateTime.UtcNow))
                throw new InvalidOperationException("Cannot purchase past tours");

            // Validate bonus points don't exceed total price
            if (BonusPointsUsed > TotalPrice.Amount)
                throw new InvalidOperationException("Bonus points cannot exceed total price");
        }

        public Money GetFinalPrice()
        {
            var finalAmount = Math.Max(0, TotalPrice.Amount - BonusPointsUsed);
            return new Money(finalAmount, TotalPrice.Currency);
        }
    }
}
