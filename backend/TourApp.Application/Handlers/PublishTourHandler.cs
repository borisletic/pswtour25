using MediatR;
using TourApp.Application.Commands;
using TourApp.Application.Services;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class PublishTourHandler : IRequestHandler<PublishTourCommand, CommandResult>
    {
        private readonly ITourRepository _tourRepository;
        private readonly ITourRecommendationService _tourRecommendationService;

        public PublishTourHandler(
            ITourRepository tourRepository,
            ITourRecommendationService tourRecommendationService)
        {
            _tourRepository = tourRepository;
            _tourRecommendationService = tourRecommendationService;
        }

        public async Task<CommandResult> Handle(PublishTourCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var tour = await _tourRepository.GetWithKeyPointsAsync(request.TourId);
            if (tour == null)
            {
                result.Errors.Add("Tour not found");
                return result;
            }

            if (tour.GuideId != request.GuideId)
            {
                result.Errors.Add("You can only publish your own tours");
                return result;
            }

            try
            {
                tour.Publish();
                _tourRepository.Update(tour);
                await _tourRepository.SaveChangesAsync();

                try
                {
                    Console.WriteLine($"=== TOUR PUBLISHED SUCCESSFULLY ===");
                    Console.WriteLine($"Tour ID: {tour.Id}");
                    Console.WriteLine($"Tour Name: {tour.Name}");
                    Console.WriteLine($"Tour Category: {tour.Category}");
                    Console.WriteLine($"Sending recommendations...");

                    await _tourRecommendationService.SendRecommendationsForNewTour(tour);

                    Console.WriteLine("Recommendations process completed!");
                    Console.WriteLine($"=== END PUBLISH PROCESS ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ ERROR sending recommendations: {ex.Message}");
                    Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                    // Ne prekidaj publish zbog greške u preporukama
                }

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