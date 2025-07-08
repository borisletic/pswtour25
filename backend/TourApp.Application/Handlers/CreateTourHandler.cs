using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Domain.ValueObjects;

namespace TourApp.Application.Handlers
{
    public class CreateTourHandler : IRequestHandler<CreateTourCommand, CreateTourResult>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IGuideRepository _guideRepository;

        public CreateTourHandler(ITourRepository tourRepository, IGuideRepository guideRepository)
        {
            _tourRepository = tourRepository;
            _guideRepository = guideRepository;
        }

        public async Task<CreateTourResult> Handle(CreateTourCommand request, CancellationToken cancellationToken)
        {
            var result = new CreateTourResult();

            // Validate guide exists
            var guide = await _guideRepository.GetByIdAsync(request.GuideId);
            if (guide == null)
            {
                result.Errors.Add("Guide not found");
                return result;
            }

            // Create tour
            var price = new Money(request.Price);
            var tour = new Tour(
                request.GuideId,
                request.Name,
                request.Description,
                request.Difficulty,
                request.Category,
                price,
                request.ScheduledDate
            );

            await _tourRepository.AddAsync(tour);
            await _tourRepository.SaveChangesAsync();

            // Update guide's tours
            guide.AddTour(tour);
            _guideRepository.Update(guide);
            await _guideRepository.SaveChangesAsync();

            result.Success = true;
            result.TourId = tour.Id;
            return result;
        }
    }
}
