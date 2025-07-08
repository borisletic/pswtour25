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
    public class ReturnProblemToPendingHandler : IRequestHandler<ReturnProblemToPendingCommand, CommandResult>
    {
        private readonly IProblemRepository _problemRepository;

        public ReturnProblemToPendingHandler(IProblemRepository problemRepository)
        {
            _problemRepository = problemRepository;
        }

        public async Task<CommandResult> Handle(ReturnProblemToPendingCommand request, CancellationToken cancellationToken)
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
                problem.ReturnToPending();
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
