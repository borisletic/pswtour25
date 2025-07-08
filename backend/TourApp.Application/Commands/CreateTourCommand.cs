using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Enums;

namespace TourApp.Application.Commands
{
    public class CreateTourCommand : IRequest<CreateTourResult>
    {
        public Guid GuideId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TourDifficulty Difficulty { get; set; }
        public Interest Category { get; set; }
        public decimal Price { get; set; }
        public DateTime ScheduledDate { get; set; }
    }

    public class CreateTourResult
    {
        public bool Success { get; set; }
        public Guid TourId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
