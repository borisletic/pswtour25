using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.DTOs;
using TourApp.Application.Queries;
using TourApp.Domain.Enums;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class GetProblemsUnderReviewHandler : IRequestHandler<GetProblemsUnderReviewQuery, List<ProblemDto>>
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITouristRepository _touristRepository;

        public GetProblemsUnderReviewHandler(
            IProblemRepository problemRepository,
            ITourRepository tourRepository,
            ITouristRepository touristRepository)
        {
            _problemRepository = problemRepository;
            _tourRepository = tourRepository;
            _touristRepository = touristRepository;
        }

        public async Task<List<ProblemDto>> Handle(GetProblemsUnderReviewQuery request, CancellationToken cancellationToken)
        {
            var problems = await _problemRepository.GetByStatusAsync(ProblemStatus.UnderReview);
            var problemDtos = new List<ProblemDto>();

            foreach (var problem in problems)
            {
                var tour = await _tourRepository.GetByIdAsync(problem.TourId);
                var tourist = await _touristRepository.GetByIdAsync(problem.TouristId);

                problemDtos.Add(new ProblemDto
                {
                    Id = problem.Id,
                    TourId = problem.TourId,
                    TourName = tour?.Name ?? "Unknown Tour",
                    TouristId = problem.TouristId,
                    TouristName = $"{tourist?.FirstName} {tourist?.LastName}",
                    Title = problem.Title,
                    Description = problem.Description,
                    Status = problem.Status.ToString(),
                    CreatedAt = problem.CreatedAt,
                    Events = problem.Events.Select(e => new ProblemEventDto
                    {
                        OccurredAt = e.OccurredAt,
                        Type = e.GetType().Name,
                        OldStatus = (e as Domain.Events.ProblemStatusChangedEvent)?.OldStatus.ToString(),
                        NewStatus = (e as Domain.Events.ProblemStatusChangedEvent)?.NewStatus.ToString()
                    }).ToList()
                });
            }

            return problemDtos.OrderByDescending(p => p.CreatedAt).ToList();
        }
    }
}
