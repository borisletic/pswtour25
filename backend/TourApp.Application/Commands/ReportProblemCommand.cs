using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Commands
{
    public class ReportProblemCommand : IRequest<ReportProblemResult>
    {
        public Guid TourId { get; set; }
        public Guid TouristId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ReportProblemResult : CommandResult
    {
        public Guid ProblemId { get; set; }
    }
}
