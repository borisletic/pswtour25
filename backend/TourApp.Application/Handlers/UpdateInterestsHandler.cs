using MediatR;
using TourApp.Application.Commands;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class UpdateInterestsHandler : IRequestHandler<UpdateInterestsCommand, CommandResult>
    {
        private readonly ITouristRepository _touristRepository;

        public UpdateInterestsHandler(ITouristRepository touristRepository)
        {
            _touristRepository = touristRepository;
        }

        public async Task<CommandResult> Handle(UpdateInterestsCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var tourist = await _touristRepository.GetByIdAsync(request.TouristId);
            if (tourist == null)
            {
                result.Errors.Add("Tourist not found");
                return result;
            }

            try
            {
                tourist.UpdateInterests(request.Interests);
                _touristRepository.Update(tourist);
                await _touristRepository.SaveChangesAsync();
                result.Success = true;
            }
            catch (ArgumentException ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}