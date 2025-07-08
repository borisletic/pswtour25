using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class ReportProblemHandler : IRequestHandler<ReportProblemCommand, ReportProblemResult>
    {
        private readonly IProblemRepository _problemRepository;
        private readonly IPurchaseRepository _purchaseRepository;

        public ReportProblemHandler(
            IProblemRepository problemRepository,
            IPurchaseRepository purchaseRepository)
        {
            _problemRepository = problemRepository;
            _purchaseRepository = purchaseRepository;
        }

        public async Task<ReportProblemResult> Handle(ReportProblemCommand request, CancellationToken cancellationToken)
        {
            var result = new ReportProblemResult();

            // Check if tourist has purchased the tour
            var hasPurchased = await _purchaseRepository.HasTouristPurchasedTourAsync(
                request.TouristId, request.TourId);

            if (!hasPurchased)
            {
                result.Errors.Add("You can only report problems for tours you have purchased");
                return result;
            }

            // Create problem
            var problem = new Problem(
                request.TourId,
                request.TouristId,
                request.Title,
                request.Description
            );

            await _problemRepository.AddAsync(problem);
            await _problemRepository.SaveChangesAsync();

            result.Success = true;
            result.ProblemId = problem.Id;

            return result;
        }
    }
}
