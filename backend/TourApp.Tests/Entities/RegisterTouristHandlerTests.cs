using Moq;
using TourApp.Domain.Enums;
using TourApp.Domain.Repositories;
using FluentAssertions;
using TourApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using TourApp.Application.Handlers;
using TourApp.Application.Services;
using TourApp.Application.Commands;

namespace TourApp.Tests.Entities
{
    public class RegisterTouristHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly RegisterTouristHandler _handler;

        public RegisterTouristHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _handler = new RegisterTouristHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object
            );
        }

        [Fact]
        public async Task Handle_WithNewUser_ReturnsSuccess()
        {
            // Arrange
            var command = new RegisterTouristCommand
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "SecurePass123!",
                FirstName = "New",
                LastName = "User",
                Interests = new List<Interest> { Interest.Nature, Interest.Sport }
            };

            _userRepositoryMock
                .Setup(x => x.ExistsAsync(command.Username, command.Email))
                .ReturnsAsync(false);

            _passwordHasherMock
                .Setup(x => x.HashPassword(command.Password))
                .Returns("hashedPassword");

            _userRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.TouristId.Should().NotBeNullOrEmpty();
            result.Errors.Should().BeEmpty();

            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tourist>()), Times.Once);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithExistingUser_ReturnsError()
        {
            // Arrange
            var command = new RegisterTouristCommand
            {
                Username = "existinguser",
                Email = "existing@example.com",
                Password = "SecurePass123!",
                FirstName = "Existing",
                LastName = "User",
                Interests = new List<Interest> { Interest.Nature }
            };

            _userRepositoryMock
                .Setup(x => x.ExistsAsync(command.Username, command.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Username or email already exists");

            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tourist>()), Times.Never);
            _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }
    }
}
