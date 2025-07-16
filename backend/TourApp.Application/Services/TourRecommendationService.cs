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
            Console.WriteLine($"=== STARTING RECOMMENDATION PROCESS ===");
            Console.WriteLine($"Tour: {tour.Name}");
            Console.WriteLine($"Category: {tour.Category}");

            try
            {
                // Find tourists interested in this category
                Console.WriteLine($"Searching for tourists interested in: {tour.Category}");
                var interestedTourists = await _touristRepository.GetByInterestsAsync(
                    new List<Interest> { tour.Category });

                Console.WriteLine($"Found {interestedTourists.Count()} tourists with matching interests");

                if (!interestedTourists.Any())
                {
                    Console.WriteLine("⚠️ No tourists found with matching interests!");
                    return;
                }

                var unblocked = interestedTourists.Where(t => !t.IsBlocked).ToList();
                Console.WriteLine($"Of those, {unblocked.Count} are not blocked");

                foreach (var tourist in unblocked)
                {
                    Console.WriteLine($"📧 Sending recommendation to: {tourist.Email} ({tourist.FirstName} {tourist.LastName})");

                    try
                    {
                        await SendRecommendationEmail(tourist, tour);
                        Console.WriteLine($"✅ Email sent successfully to: {tourist.Email}");
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"❌ Failed to send email to {tourist.Email}: {emailEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in SendRecommendationsForNewTour: {ex.Message}");
                throw;
            }

            Console.WriteLine($"=== RECOMMENDATION PROCESS COMPLETE ===");
        }

        private async Task SendRecommendationEmail(Tourist tourist, Tour tour)
        {
            Console.WriteLine($"🔧 Preparing email for {tourist.Email}...");

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
            <p><a href='http://localhost:4200/tours/{tour.Id}'>View Tour Details</a></p>
            <br>
            <p>Happy exploring!</p>
            <p>Tourist Tours Team</p>
        </body>
        </html>";

            Console.WriteLine($"📬 Calling EmailService.SendEmailAsync...");
            await _emailService.SendEmailAsync(tourist.Email, subject, body);
            Console.WriteLine($"📬 EmailService call completed for {tourist.Email}");
        }
    }
}
