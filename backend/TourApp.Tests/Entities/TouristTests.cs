using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;

namespace TourApp.Tests.Entities
{
    public class TouristTests
    {
        [Fact]
        public void Constructor_WithValidData_CreatesTourist()
        {
            // Arrange
            var username = "john_doe";
            var email = "john@example.com";
            var passwordHash = "hashedPassword";
            var firstName = "John";
            var lastName = "Doe";
            var interests = new List<Interest> { Interest.Nature, Interest.Art };

            // Act
            var tourist = new Tourist(username, email, passwordHash, firstName, lastName, interests);

            // Assert
            tourist.Should().NotBeNull();
            tourist.Username.Should().Be(username);
            tourist.Email.Should().Be(email);
            tourist.FirstName.Should().Be(firstName);
            tourist.LastName.Should().Be(lastName);
            tourist.Interests.Should().BeEquivalentTo(interests);
            tourist.BonusPoints.Should().Be(0);
            tourist.InvalidProblemsCount.Should().Be(0);
            tourist.IsBlocked.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithNoInterests_ThrowsException()
        {
            // Arrange
            var interests = new List<Interest>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Tourist("john", "john@example.com", "hash", "John", "Doe", interests)
            );
        }

        [Fact]
        public void AddBonusPoints_WithPositiveAmount_IncreasesBonusPoints()
        {
            // Arrange
            var tourist = CreateValidTourist();
            var pointsToAdd = 50m;

            // Act
            tourist.AddBonusPoints(pointsToAdd);

            // Assert
            tourist.BonusPoints.Should().Be(pointsToAdd);
        }

        [Fact]
        public void AddBonusPoints_WithNegativeAmount_ThrowsException()
        {
            // Arrange
            var tourist = CreateValidTourist();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => tourist.AddBonusPoints(-10));
        }

        [Fact]
        public void UseBonusPoints_WithAvailablePoints_ReturnsUsedAmount()
        {
            // Arrange
            var tourist = CreateValidTourist();
            tourist.AddBonusPoints(100);

            // Act
            var usedPoints = tourist.UseBonusPoints(60);

            // Assert
            usedPoints.Should().Be(60);
            tourist.BonusPoints.Should().Be(40);
        }

        [Fact]
        public void UseBonusPoints_WithMoreThanAvailable_ReturnsAvailableAmount()
        {
            // Arrange
            var tourist = CreateValidTourist();
            tourist.AddBonusPoints(50);

            // Act
            var usedPoints = tourist.UseBonusPoints(100);

            // Assert
            usedPoints.Should().Be(50);
            tourist.BonusPoints.Should().Be(0);
        }

        [Fact]
        public void IsMalicious_With10InvalidProblems_ReturnsTrue()
        {
            // Arrange
            var tourist = CreateValidTourist();
            for (int i = 0; i < 10; i++)
            {
                tourist.IncrementInvalidProblems();
            }

            // Act
            var isMalicious = tourist.IsMalicious();

            // Assert
            isMalicious.Should().BeTrue();
        }

        [Fact]
        public void UpdateInterests_WithValidInterests_UpdatesSuccessfully()
        {
            // Arrange
            var tourist = CreateValidTourist();
            var newInterests = new List<Interest> { Interest.Sport, Interest.Food };

            // Act
            tourist.UpdateInterests(newInterests);

            // Assert
            tourist.Interests.Should().BeEquivalentTo(newInterests);
        }

        private Tourist CreateValidTourist()
        {
            return new Tourist(
                "john_doe",
                "john@example.com",
                "hashedPassword",
                "John",
                "Doe",
                new List<Interest> { Interest.Nature }
            );
        }
    }
}
