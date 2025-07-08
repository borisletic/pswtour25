using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;
using TourApp.Domain.ValueObjects;

namespace TourApp.Tests.Entities
{
    public class TourTests
    {
        [Fact]
        public void Constructor_WithValidData_CreatesTour()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var name = "City Walking Tour";
            var description = "Explore the historic city center";
            var difficulty = TourDifficulty.Easy;
            var category = Interest.Art;
            var price = new Money(50, "EUR");
            var scheduledDate = DateTime.UtcNow.AddDays(7);

            // Act
            var tour = new Tour(guideId, name, description, difficulty, category, price, scheduledDate);

            // Assert
            tour.Should().NotBeNull();
            tour.Name.Should().Be(name);
            tour.Description.Should().Be(description);
            tour.Difficulty.Should().Be(difficulty);
            tour.Category.Should().Be(category);
            tour.Price.Should().Be(price);
            tour.ScheduledDate.Should().Be(scheduledDate);
            tour.Status.Should().Be(TourStatus.Draft);
        }

        [Fact]
        public void Publish_WithLessThan2KeyPoints_ThrowsException()
        {
            // Arrange
            var tour = CreateValidTour();
            tour.AddKeyPoint(CreateKeyPoint(tour.Id));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => tour.Publish());
        }

        [Fact]
        public void Publish_With2OrMoreKeyPoints_ChangesStatusToPublished()
        {
            // Arrange
            var tour = CreateValidTour();
            tour.AddKeyPoint(CreateKeyPoint(tour.Id, 1));
            tour.AddKeyPoint(CreateKeyPoint(tour.Id, 2));

            // Act
            tour.Publish();

            // Assert
            tour.Status.Should().Be(TourStatus.Published);
        }

        [Fact]
        public void Cancel_Within24HoursOfTour_ThrowsException()
        {
            // Arrange
            var tour = new Tour(
                Guid.NewGuid(),
                "Tour",
                "Description",
                TourDifficulty.Easy,
                Interest.Nature,
                new Money(50),
                DateTime.UtcNow.AddHours(12) // 12 hours from now
            );

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => tour.Cancel());
        }

        [Fact]
        public void Cancel_MoreThan24HoursBeforeTour_ChangesStatusToCancelled()
        {
            // Arrange
            var tour = new Tour(
                Guid.NewGuid(),
                "Tour",
                "Description",
                TourDifficulty.Easy,
                Interest.Nature,
                new Money(50),
                DateTime.UtcNow.AddDays(2) // 2 days from now
            );

            // Act
            tour.Cancel();

            // Assert
            tour.Status.Should().Be(TourStatus.Cancelled);
        }

        [Fact]
        public void GetAverageRating_WithNoRatings_ReturnsZero()
        {
            // Arrange
            var tour = CreateValidTour();

            // Act
            var averageRating = tour.GetAverageRating();

            // Assert
            averageRating.Should().Be(0);
        }

        [Fact]
        public void GetAverageRating_WithMultipleRatings_ReturnsCorrectAverage()
        {
            // Arrange
            var tour = CreateValidTour();
            var touristId1 = Guid.NewGuid();
            var touristId2 = Guid.NewGuid();

            tour.AddRating(new Rating(tour.Id, touristId1, 4, "Good tour"));
            tour.AddRating(new Rating(tour.Id, touristId2, 5, "Excellent!"));

            // Act
            var averageRating = tour.GetAverageRating();

            // Assert
            averageRating.Should().Be(4.5);
        }

        private Tour CreateValidTour()
        {
            return new Tour(
                Guid.NewGuid(),
                "Test Tour",
                "Test Description",
                TourDifficulty.Medium,
                Interest.Nature,
                new Money(100),
                DateTime.UtcNow.AddDays(7)
            );
        }

        private KeyPoint CreateKeyPoint(Guid tourId, int order = 1)
        {
            return new KeyPoint(
                tourId,
                $"Point {order}",
                "Description",
                new Location(45.0, 15.0),
                null,
                order
            );
        }
    }
}
