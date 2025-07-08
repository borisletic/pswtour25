using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Domain.Repositories;
using TourApp.Domain.Entities;

namespace TourApp.Application.Handlers
{
    public class SendProblemForReviewHandler : IRequestHandler<SendProblemForReviewCommand, CommandResult>
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ITourRepository _tourRepository;

        public SendProblemForReviewHandler(
            IProblemRepository problemRepository,
            ITourRepository tourRepository)
        {
            _problemRepository = problemRepository;
            _tourRepository = tourRepository;
        }

        public async Task<CommandResult> Handle(SendProblemForReviewCommand request, CancellationToken cancellationToken)
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
                result.Errors.Add("You can only send problems for review for your own tours");
                return result;
            }

            try
            {
                problem.SendForReview();
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
