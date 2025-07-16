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
    public class GetPurchaseHistoryHandler : IRequestHandler<GetPurchaseHistoryQuery, List<PurchaseHistoryDto>>
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IGuideRepository _guideRepository;
        private readonly IRatingRepository _ratingRepository;

        public GetPurchaseHistoryHandler(
            IPurchaseRepository purchaseRepository,
            IGuideRepository guideRepository,
            IRatingRepository ratingRepository)
        {
            _purchaseRepository = purchaseRepository;
            _guideRepository = guideRepository;
            _ratingRepository = ratingRepository;
        }

        public async Task<List<PurchaseHistoryDto>> Handle(GetPurchaseHistoryQuery request, CancellationToken cancellationToken)
        {
            var purchases = await _purchaseRepository.GetByTouristAsync(request.TouristId);
            var purchaseDtos = new List<PurchaseHistoryDto>();

            foreach (var purchase in purchases)
            {
                var tourDtos = new List<TourDto>();

                foreach (var tour in purchase.Tours)
                {
                    var guide = await _guideRepository.GetByIdAsync(tour.GuideId);
                    var hasRated = await _ratingRepository.ExistsAsync(request.TouristId, tour.Id);

                    var tourDto = new TourDto
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
                        HasRated = hasRated, 
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

                    tourDtos.Add(tourDto);
                }

                purchaseDtos.Add(new PurchaseHistoryDto
                {
                    Id = purchase.Id,
                    PurchasedAt = purchase.PurchasedAt,
                    Tours = tourDtos,
                    TotalPrice = purchase.TotalPrice.Amount,
                    BonusPointsUsed = purchase.BonusPointsUsed,
                    Currency = purchase.TotalPrice.Currency
                });
            }

            return purchaseDtos.OrderByDescending(p => p.PurchasedAt).ToList();
        }
    }
}
