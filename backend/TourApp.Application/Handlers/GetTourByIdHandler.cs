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
    public class GetTourByIdHandler : IRequestHandler<GetTourByIdQuery, TourDto>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IGuideRepository _guideRepository;

        public GetTourByIdHandler(ITourRepository tourRepository, IGuideRepository guideRepository)
        {
            _tourRepository = tourRepository;
            _guideRepository = guideRepository;
        }

        public async Task<TourDto> Handle(GetTourByIdQuery request, CancellationToken cancellationToken)
        {
            var tour = await _tourRepository.GetWithKeyPointsAsync(request.TourId);
            if (tour == null)
                return null;

            var tourWithRatings = await _tourRepository.GetWithRatingsAsync(request.TourId);
            var guide = await _guideRepository.GetByIdAsync(tour.GuideId);

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
                AverageRating = tourWithRatings.GetAverageRating(),
                RatingsCount = tourWithRatings.Ratings.Count,
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
