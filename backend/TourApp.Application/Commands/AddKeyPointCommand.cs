using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Commands
{
    public class AddKeyPointCommand : IRequest<AddKeyPointResult>
    {
        public Guid TourId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
    }

    public class AddKeyPointResult
    {
        public bool Success { get; set; }
        public Guid KeyPointId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
