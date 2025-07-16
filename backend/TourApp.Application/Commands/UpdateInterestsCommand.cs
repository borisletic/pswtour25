using MediatR;
using TourApp.Domain.Enums;

namespace TourApp.Application.Commands
{
    public class UpdateInterestsCommand : IRequest<CommandResult>
    {
        public Guid TouristId { get; set; }
        public List<Interest> Interests { get; set; } = new List<Interest>();
    }
}