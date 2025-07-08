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
    public class RejectProblemHandler : IRequestHandler<RejectProblemCommand, CommandResult>
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ITouristRepository _touristRepository;

        public RejectProblemHandler(
            IProblemRepository problemRepository,
            ITouristRepository touristRepository)
        {
            _problemRepository = problemRepository;
            _touristRepository = touristRepository;
        }

        public async Task<CommandResult> Handle(RejectProblemCommand request, CancellationToken cancellationToken)
        {
            var result = new CommandResult();

            var problem = await _problemRepository.GetWithEventsAsync(request.ProblemId);
            if (problem == null)
            {
                result.Errors.Add("Problem not found");
                return result;
            }

            try
            {
                problem.Reject();
                _problemRepository.Update(problem);

                // Increment tourist's invalid problems count
                var tourist = await _touristRepository.GetByIdAsync(problem.TouristId);
                tourist.IncrementInvalidProblems();
                _touristRepository.Update(tourist);

                await _problemRepository.SaveChangesAsync();
                await _touristRepository.SaveChangesAsync();

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
