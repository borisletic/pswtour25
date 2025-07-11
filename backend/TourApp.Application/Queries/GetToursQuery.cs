using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.DTOs;
using TourApp.Domain.Enums;

namespace TourApp.Application.Queries
{
    public class GetToursQuery : IRequest<List<TourDto>>
    {
        public Interest? Category { get; set; }
        public TourDifficulty? Difficulty { get; set; }
        public string? Status { get; set; }
        public Guid? GuideId { get; set; }
        public bool? RewardedGuidesOnly { get; set; }
    }
}
