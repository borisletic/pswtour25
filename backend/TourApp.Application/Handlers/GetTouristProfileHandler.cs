using MediatR;
using TourApp.Application.DTOs;
using TourApp.Application.Queries;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class GetTouristProfileHandler : IRequestHandler<GetTouristProfileQuery, TouristProfileDto>
    {
        private readonly ITouristRepository _touristRepository;

        public GetTouristProfileHandler(ITouristRepository touristRepository)
        {
            _touristRepository = touristRepository;
        }

        public async Task<TouristProfileDto> Handle(GetTouristProfileQuery request, CancellationToken cancellationToken)
        {
            var tourist = await _touristRepository.GetByIdAsync(request.TouristId);

            if (tourist == null)
                throw new ArgumentException("Tourist not found");

            return new TouristProfileDto
            {
                Id = tourist.Id,
                Username = tourist.Username,
                Email = tourist.Email,
                FirstName = tourist.FirstName,
                LastName = tourist.LastName,
                Interests = tourist.Interests.ToList(),
                BonusPoints = tourist.BonusPoints
            };
        }
    }
}