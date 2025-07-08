using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.DTOs;
using TourApp.Application.Queries;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class GetToursHandler : IRequestHandler<GetToursQuery, List<TourDto>>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IGuideRepository _guideRepository;

        public GetToursHandler(ITourRepository tourRepository, IGuideRepository guideRepository)
        {
            _tourRepository = tourRepository;
            _guideRepository = guideRepository;
        }

        public async Task<List<TourDto>> Handle(GetToursQuery request, CancellationToken cancellationToken)
        {
            var tours = await _tourRepository.GetPublishedToursAsync();

            // Apply filters
            if (request.Category.HasValue)
            {
                tours = tours.Where(t => t.Category == request.Category.Value);
            }

            if (request.Difficulty.HasValue)
            {
                tours = tours.Where(t => t.Difficulty == request.Difficulty.Value);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                tours = tours.Where(t => t.Status.ToString() == request.Status);
            }

            if (request.GuideId.HasValue)
            {
                tours = tours.Where(t => t.GuideId == request.GuideId.Value);
            }

            if (request.RewardedGuidesOnly == true)
            {
                var rewardedGuides = await _guideRepository.GetRewardedGuidesAsync();
                var rewardedGuideIds = rewardedGuides.Select(g => g.Id).ToHashSet();
                tours = tours.Where(t => rewardedGuideIds.Contains(t.GuideId));
            }

            // Map to DTOs
            var tourDtos = new List<TourDto>();
            foreach (var tour in tours)
            {
                var guide = await _guideRepository.GetByIdAsync(tour.GuideId);
                tourDtos.Add(MapToDto(tour, guide));
            }

            return tourDtos;
        }

        private TourDto MapToDto(Domain.Entities.Tour tour, Domain.Entities.Guide guide)
        {
            return new TourDto
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Difficulty = tour.Difficulty.ToString(),
                Category = tour.Category.ToString(),
                Price = tour.Price.Amount,
                Currency = tour.Price.Currency,
                ScheduledDate = tour.ScheduledDate,
                Status = tour.Status.ToString(),
                AverageRating = tour.GetAverageRating(),
                RatingsCount = tour.Ratings.Count,
                KeyPoints = tour.KeyPoints.Select(kp => new KeyPointDto
                {
                    Id = kp.Id,
                    Name = kp.Name,
                    Description = kp.Description,
                    Latitude = kp.Location.Latitude,
                    Longitude = kp.Location.Longitude,
                    ImageUrl = kp.ImageUrl,
                    Order = kp.Order
                }).ToList(),
                Guide = new GuideInfoDto
                {
                    Id = guide.Id,
                    FirstName = guide.FirstName,
                    LastName = guide.LastName,
                    IsRewarded = guide.IsRewarded
                }
            };
        }
    }
}
