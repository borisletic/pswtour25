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
    public class ResolveProblemHandler : IRequestHandler<ResolveProblemCommand, CommandResult>
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ITourRepository _tourRepository;

        public ResolveProblemHandler(
            IProblemRepository problemRepository,
            ITourRepository tourRepository)
        {
            _problemRepository = problemRepository;
            _tourRepository = tourRepository;
        }

        public async Task<CommandResult> Handle(ResolveProblemCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var problem = await _problemRepository.GetWithEventsAsync(request.ProblemId);
            if (problem == null)
            {
                result.Errors.Add("Problem not found");
                return result;
            }

            // Verify the guide owns the tour
            var tour = await _tourRepository.GetByIdAsync(problem.TourId);
            if (tour.GuideId != request.GuideId)
            {
                result.Errors.Add("You can only resolve problems for your own tours");
                return result;
            }

            try
            {
                problem.MarkAsResolved();
                _problemRepository.Update(problem);
                await _problemRepository.SaveChangesAsync();
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
