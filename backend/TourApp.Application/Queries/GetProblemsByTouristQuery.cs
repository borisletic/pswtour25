using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.DTOs;

namespace TourApp.Application.Queries
{
    public class GetProblemsByTouristQuery : IRequest<List<ProblemDto>>
    {
        public Guid TouristId { get; set; }
    }
}
