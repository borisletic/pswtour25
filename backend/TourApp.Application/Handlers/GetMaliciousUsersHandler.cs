using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.DTOs;
using TourApp.Application.Queries;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class GetMaliciousUsersHandler : IRequestHandler<GetMaliciousUsersQuery, List<MaliciousUserDto>>
    {
        private readonly ITouristRepository _touristRepository;
        private readonly IGuideRepository _guideRepository;

        public GetMaliciousUsersHandler(ITouristRepository touristRepository, IGuideRepository guideRepository)
        {
            _touristRepository = touristRepository;
            _guideRepository = guideRepository;
        }

        public async Task<List<MaliciousUserDto>> Handle(GetMaliciousUsersQuery request, CancellationToken cancellationToken)
        {
            var maliciousUsers = new List<MaliciousUserDto>();

            // Get malicious tourists
            var maliciousTourists = await _touristRepository.GetMaliciousTouristsAsync();
            foreach (var tourist in maliciousTourists)
            {
                maliciousUsers.Add(new MaliciousUserDto
                {
                    Id = tourist.Id,
                    Username = tourist.Username,
                    Email = tourist.Email,
                    Type = "Tourist",
                    Reason = "Invalid problems reported",
                    Count = tourist.InvalidProblemsCount,
                    IsBlocked = tourist.IsBlocked
                });
            }

            // Get malicious guides
            var maliciousGuides = await _guideRepository.GetMaliciousGuidesAsync();
            foreach (var guide in maliciousGuides)
            {
                maliciousUsers.Add(new MaliciousUserDto
                {
                    Id = guide.Id,
                    Username = guide.Username,
                    Email = guide.Email,
                    Type = "Guide",
                    Reason = "Tours cancelled",
                    Count = guide.CancelledToursCount,
                    IsBlocked = guide.IsBlocked
                });
            }

            return maliciousUsers.OrderByDescending(u => u.Count).ToList();
        }
    }
}
