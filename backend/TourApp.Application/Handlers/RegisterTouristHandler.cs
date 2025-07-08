using MediatR;
using TourApp.Application.Commands;
using TourApp.Application.Services;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class RegisterTouristHandler : IRequestHandler<RegisterTouristCommand, RegisterTouristResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterTouristHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterTouristResult> Handle(RegisterTouristCommand request, CancellationToken cancellationToken)
        {
            var result = new RegisterTouristResult();

            // Validate if user already exists
            if (await _userRepository.ExistsAsync(request.Username, request.Email))
            {
                result.Errors.Add("Username or email already exists");
                return result;
            }

            // Create new tourist
            var passwordHash = _passwordHasher.HashPassword(request.Password);
            var tourist = new Tourist(
                request.Username,
                request.Email,
                passwordHash,
                request.FirstName,
                request.LastName,
                request.Interests
            );

            await _userRepository.AddAsync(tourist);
            await _userRepository.SaveChangesAsync();

            result.Success = true;
            result.TouristId = tourist.Id.ToString();
            return result;
        }
    }
}
