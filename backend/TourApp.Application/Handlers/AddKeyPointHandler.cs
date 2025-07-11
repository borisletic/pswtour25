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
        private readonly IKeyPointRepository _keyPointRepository;

        public AddKeyPointHandler(ITourRepository tourRepository, IKeyPointRepository keyPointRepository)
        {
            _tourRepository = tourRepository;
            _keyPointRepository = keyPointRepository;
        }

        public async Task<AddKeyPointResult> Handle(AddKeyPointCommand request, CancellationToken cancellationToken)
        {
            var result = new AddKeyPointResult();

            try
            {
                // Just verify the tour exists (lightweight check)
                var tourExists = await _tourRepository.GetByIdAsync(request.TourId);
                if (tourExists == null)
                {
                    result.Errors.Add("Tour not found");
                    return result;
                }

                // Create the key point directly
                var location = new Location(request.Latitude, request.Longitude);
                var keyPoint = new KeyPoint(
                    request.TourId,
                    request.Name,
                    request.Description,
                    location,
                    request.ImageUrl,
                    request.Order
                );

                // Add key point directly to database without touching Tour entity
                await _keyPointRepository.AddAsync(keyPoint);
                await _keyPointRepository.SaveChangesAsync();

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