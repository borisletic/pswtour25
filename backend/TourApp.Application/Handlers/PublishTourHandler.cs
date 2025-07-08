using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class PublishTourHandler : IRequestHandler<PublishTourCommand, CommandResult>
    {
        private readonly ITourRepository _tourRepository;

        public PublishTourHandler(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public async Task<CommandResult> Handle(PublishTourCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var tour = await _tourRepository.GetWithKeyPointsAsync(request.TourId);
            if (tour == null)
            {
                result.Errors.Add("Tour not found");
                return result;
            }

            if (tour.GuideId != request.GuideId)
            {
                result.Errors.Add("You can only publish your own tours");
                return result;
            }

            try
            {
                tour.Publish();
                _tourRepository.Update(tour);
                await _tourRepository.SaveChangesAsync();
                result.Success = true;
            }
            catch (InvalidOperationException ex)
            {
                result.Errors.Add(ex.Message);
            }

            return result;
        }
    }
}
