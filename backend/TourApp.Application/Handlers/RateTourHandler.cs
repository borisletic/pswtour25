using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class RateTourHandler : IRequestHandler<RateTourCommand, CommandResult>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IRatingRepository _ratingRepository;

        public RateTourHandler(
            ITourRepository tourRepository,
            IPurchaseRepository purchaseRepository,
            IRatingRepository ratingRepository)
        {
            _tourRepository = tourRepository;
            _purchaseRepository = purchaseRepository;
            _ratingRepository = ratingRepository;
        }

        public async Task<CommandResult> Handle(RateTourCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            // Check if tourist has purchased the tour
            var hasPurchased = await _purchaseRepository.HasTouristPurchasedTourAsync(
                request.TouristId, request.TourId);

            if (!hasPurchased)
            {
                result.Errors.Add("You can only rate tours you have purchased");
                return result;
            }

            // Check if tour date has passed
            var tour = await _tourRepository.GetByIdAsync(request.TourId);
            if (tour.ScheduledDate > DateTime.UtcNow)
            {
                result.Errors.Add("You can only rate tours after they have been completed");
                return result;
            }

            // Check if rating already exists
            var existingRating = await _ratingRepository.GetByTouristAndTourAsync(
                request.TouristId, request.TourId);

            if (existingRating != null)
            {
                result.Errors.Add("You have already rated this tour");
                return result;
            }

            // Check if within 30 days
            if (tour.ScheduledDate < DateTime.UtcNow.AddDays(-30))
            {
                result.Errors.Add("Rating period has expired (30 days after tour)");
                return result;
            }

            try
            {
                // Create rating
                var rating = new Rating(request.TourId, request.TouristId, request.Score, request.Comment);

                // Add rating to tour
                tour.AddRating(rating);

                await _ratingRepository.AddAsync(rating);
                _tourRepository.Update(tour);

                await _ratingRepository.SaveChangesAsync();
                await _tourRepository.SaveChangesAsync();

                result.Success = true;
            }
            catch (ArgumentException ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
