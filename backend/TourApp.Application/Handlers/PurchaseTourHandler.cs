using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Application.Services;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Domain.ValueObjects;

namespace TourApp.Application.Handlers
{
    public class PurchaseTourHandler : IRequestHandler<PurchaseTourCommand, PurchaseTourResult>
    {
        private readonly ITourRepository _tourRepository;
        private readonly ITouristRepository _touristRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IEmailService _emailService;

        public PurchaseTourHandler(
            ITourRepository tourRepository,
            ITouristRepository touristRepository,
            IPurchaseRepository purchaseRepository,
            IEmailService emailService)
        {
            _tourRepository = tourRepository;
            _touristRepository = touristRepository;
            _purchaseRepository = purchaseRepository;
            _emailService = emailService;
        }

        public async Task<PurchaseTourResult> Handle(PurchaseTourCommand request, CancellationToken cancellationToken)
        {
            var result = new PurchaseTourResult();

            // Get tourist
            var tourist = await _touristRepository.GetByIdAsync(request.TouristId);
            if (tourist == null)
            {
                result.Errors.Add("Tourist not found");
                return result;
            }

            // Get tours
            var tours = new List<Tour>();
            foreach (var tourId in request.TourIds)
            {
                var tour = await _tourRepository.GetByIdAsync(tourId);
                if (tour == null)
                {
                    result.Errors.Add($"Tour {tourId} not found");
                    return result;
                }
                tours.Add(tour);
            }

            // Calculate total price
            var totalAmount = tours.Sum(t => t.Price.Amount);
            var totalPrice = new Money(totalAmount, "EUR");

            // Apply bonus points
            var bonusPointsToUse = Math.Min(request.BonusPointsToUse, tourist.BonusPoints);
            bonusPointsToUse = Math.Min(bonusPointsToUse, totalAmount);

            // Create purchase
            var purchase = new Purchase(tourist.Id, tours, totalPrice, bonusPointsToUse);

            // Update tourist's bonus points
            if (bonusPointsToUse > 0)
            {
                tourist.UseBonusPoints(bonusPointsToUse);
                _touristRepository.Update(tourist);
            }

            // Save purchase
            await _purchaseRepository.AddAsync(purchase);
            await _purchaseRepository.SaveChangesAsync();
            await _touristRepository.SaveChangesAsync();

            // Send confirmation email
            await _emailService.SendPurchaseConfirmationAsync(tourist.Email, new PurchaseEmailData
            {
                FirstName = tourist.FirstName,
                TourNames = tours.Select(t => t.Name).ToList(),
                TotalAmount = purchase.GetFinalPrice().Amount,
                Currency = purchase.GetFinalPrice().Currency
            });

            result.Success = true;
            result.PurchaseId = purchase.Id;
            result.TotalPaid = purchase.GetFinalPrice().Amount;
            result.BonusPointsUsed = bonusPointsToUse;

            return result;
        }
    }
}
