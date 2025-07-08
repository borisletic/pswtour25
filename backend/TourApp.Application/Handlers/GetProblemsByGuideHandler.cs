using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.DTOs;
using TourApp.Application.Queries;
using TourApp.Domain.Repositories;

namespace TourApp.Application.Handlers
{
    public class GetProblemsByGuideHandler : IRequestHandler<GetProblemsByGuideQuery, List<ProblemDto>>
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ITourRepository _tourRepository;
        private readonly ITouristRepository _touristRepository;

        public GetProblemsByGuideHandler(
            IProblemRepository problemRepository,
            ITourRepository tourRepository,
            ITouristRepository touristRepository)
        {
            _problemRepository = problemRepository;
            _tourRepository = tourRepository;
            _touristRepository = touristRepository;
        }

        public async Task<List<ProblemDto>> Handle(GetProblemsByGuideQuery request, CancellationToken cancellationToken)
        {
            // Get all tours for this guide
            var guideTours = await _tourRepository.GetByGuideAsync(request.GuideId);
            var tourIds = guideTours.Select(t => t.Id).ToHashSet();

            var problemDtos = new List<ProblemDto>();

            foreach (var tourId in tourIds)
            {
                var problems = await _problemRepository.GetByTourAsync(tourId);

                foreach (var problem in problems)
                {
                    var tour = guideTours.First(t => t.Id == tourId);
                    var tourist = await _touristRepository.GetByIdAsync(problem.TouristId);

                    problemDtos.Add(new ProblemDto
                    {
                        Id = problem.Id,
                        TourId = problem.TourId,
                        TourName = tour.Name,
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
            }

            return problemDtos.OrderByDescending(p => p.CreatedAt).ToList();
        }
    }
}
