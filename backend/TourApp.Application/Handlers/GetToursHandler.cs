using MediatR;
using TourApp.Application.DTOs;
using TourApp.Application.Queries;
using TourApp.Domain.Enums;
using TourApp.Domain.Repositories;
using TourApp.Domain.ValueObjects;

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
            IEnumerable<Domain.Entities.Tour> tours;

            // If a specific guide is requested, get all their tours regardless of status
            if (request.GuideId.HasValue)
            {
                tours = await _tourRepository.GetByGuideAsync(request.GuideId.Value);
            }
            else
            {
                // For public listing, only get published tours UNLESS a specific status is requested
                if (string.IsNullOrEmpty(request.Status))
                {
                    tours = await _tourRepository.GetPublishedToursAsync();
                }
                else
                {
                    // If a specific status is requested, get all tours and filter by status
                    tours = await _tourRepository.GetAllAsync();
                }
            }

            // Apply filters
            if (request.Category.HasValue)
            {
                tours = tours.Where(t => t.Category == request.Category.Value);
            }

            if (request.Difficulty.HasValue)
            {
                tours = tours.Where(t => t.Difficulty == request.Difficulty.Value);
            }

            // Apply status filter - this is crucial for guide's "My Tours" page
            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<TourStatus>(request.Status, ignoreCase: true, out var statusEnum))
                {
                    tours = tours.Where(t => t.Status == statusEnum);
                }
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
                }).OrderBy(kp => kp.Order).ToList(),
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