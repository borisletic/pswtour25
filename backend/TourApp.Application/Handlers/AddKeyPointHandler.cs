using MediatR;
using TourApp.Application.Commands;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Domain.ValueObjects;

namespace TourApp.Application.Handlers
{
    public class AddKeyPointHandler : IRequestHandler<AddKeyPointCommand, AddKeyPointResult>
    {
        private readonly ITourRepository _tourRepository;

        public AddKeyPointHandler(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public async Task<AddKeyPointResult> Handle(AddKeyPointCommand request, CancellationToken cancellationToken)
        {
            var result = new AddKeyPointResult();

            try
            {
                // Get the tour
                var tour = await _tourRepository.GetWithKeyPointsAsync(request.TourId);
                if (tour == null)
                {
                    result.Errors.Add("Tour not found");
                    return result;
                }

                // Create the key point
                var location = new Location(request.Latitude, request.Longitude);
                var keyPoint = new KeyPoint(
                    request.TourId,
                    request.Name,
                    request.Description,
                    location,
                    request.ImageUrl,
                    request.Order
                );

                // Add key point to tour
                tour.AddKeyPoint(keyPoint);

                // Update tour in repository
                _tourRepository.Update(tour);
                await _tourRepository.SaveChangesAsync();

                result.Success = true;
                result.KeyPointId = keyPoint.Id;
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Failed to add key point: {ex.Message}");
                return result;
            }
        }
    }
}