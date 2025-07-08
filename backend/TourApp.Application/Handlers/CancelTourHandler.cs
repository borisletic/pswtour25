using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Application.Services;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class CancelTourHandler : IRequestHandler<CancelTourCommand, CommandResult>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ITouristRepository _touristRepository;
        private readonly IEmailService _emailService;
        private readonly IGuideRepository _guideRepository;

        public CancelTourHandler(
             ITourRepository tourRepository,
             IPurchaseRepository purchaseRepository,
             ITouristRepository touristRepository,
             IGuideRepository guideRepository,
             IEmailService emailService)
        {
            _tourRepository = tourRepository;
            _purchaseRepository = purchaseRepository;
            _touristRepository = touristRepository;
            _guideRepository = guideRepository;
            _emailService = emailService;
        }

        public async Task<CommandResult> Handle(CancelTourCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var tour = await _tourRepository.GetByIdAsync(request.TourId);
            if (tour == null)
            {
                result.Errors.Add("Tour not found");
                return result;
            }

            if (tour.GuideId != request.GuideId)
            {
                result.Errors.Add("You can only cancel your own tours");
                return result;
            }

            try
            {
                tour.Cancel();

                // Find all purchases containing this tour
                var purchases = await _purchaseRepository.GetByTourAsync(tour.Id);

                // Give bonus points to affected tourists
                foreach (var purchase in purchases)
                {
                    var tourist = await _touristRepository.GetByIdAsync(purchase.TouristId);
                    if (tourist != null)
                    {
                        // Add bonus points equal to tour price
                        tourist.AddBonusPoints(tour.Price.Amount);
                        _touristRepository.Update(tourist);

                        // Send notification email
                        await _emailService.SendTourCancelledNotificationAsync(
                            tourist.Email,
                            tour.Name,
                            tour.Price.Amount);
                    }
                }

                // Update guide's cancelled tours count
                var guide = await _guideRepository.GetByIdAsync(request.GuideId);
                guide.IncrementCancelledTours();
                _guideRepository.Update(guide);

                _tourRepository.Update(tour);
                await _tourRepository.SaveChangesAsync();
                await _touristRepository.SaveChangesAsync();
                await _guideRepository.SaveChangesAsync();

                result.Success = true;
            }
            catch (InvalidOperationException ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
