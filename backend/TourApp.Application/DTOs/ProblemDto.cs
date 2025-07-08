using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.DTOs
{
    public class ProblemDto
    {
        public Guid Id { get; set; }
        public Guid TourId { get; set; }
        public string TourName { get; set; }
        public Guid TouristId { get; set; }
        public string TouristName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProblemEventDto> Events { get; set; }
    }

    public class ProblemEventDto
    {
        public DateTime OccurredAt { get; set; }
        public string Type { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
    }
}
