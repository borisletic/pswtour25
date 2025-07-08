using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Services
{
    public class TourRecommendationService : ITourRecommendationService
    {
        private readonly ITouristRepository _touristRepository;
        private readonly IEmailService _emailService;

        public TourRecommendationService(
            ITouristRepository touristRepository,
            IEmailService emailService)
        {
            _touristRepository = touristRepository;
            _emailService = emailService;
        }

        public async Task SendRecommendationsForNewTour(Tour tour)
        {
            // Find tourists interested in this category
            var interestedTourists = await _touristRepository.GetByInterestsAsync(
                new List<Interest> { tour.Category });

            foreach (var tourist in interestedTourists.Where(t => !t.IsBlocked))
            {
                await SendRecommendationEmail(tourist, tour);
            }
        }

        private async Task SendRecommendationEmail(Tourist tourist, Tour tour)
        {
            var subject = "New Tour Recommendation for You!";
            var body = $@"
                <html>
                <body>
                    <h2>Hi {tourist.FirstName}!</h2>
                    <p>We have a new tour that matches your interests:</p>
                    <h3>{tour.Name}</h3>
                    <p>{tour.Description}</p>
                    <ul>
                        <li><strong>Category:</strong> {tour.Category}</li>
                        <li><strong>Difficulty:</strong> {tour.Difficulty}</li>
                        <li><strong>Date:</strong> {tour.ScheduledDate:dddd, MMMM dd, yyyy}</li>
                        <li><strong>Price:</strong> {tour.Price}</li>
                    </ul>
                    <p><a href='https://touristtours.com/tours/{tour.Id}'>View Tour Details</a></p>
                    <br>
                    <p>Happy exploring!</p>
                    <p>Tourist Tours Team</p>
                </body>
                </html>";

            await _emailService.SendEmailAsync(tourist.Email, subject, body);
        }
    }
}
