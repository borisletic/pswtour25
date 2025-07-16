using MediatR;
using TourApp.Application.DTOs;

namespace TourApp.Application.Queries
{
    public class GetTouristProfileQuery : IRequest<TouristProfileDto>
    {
        public Guid TouristId { get; set; }
    }
}