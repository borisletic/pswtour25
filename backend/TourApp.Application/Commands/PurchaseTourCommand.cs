using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.Commands
{
    public class PurchaseTourCommand : IRequest<PurchaseTourResult>
    {
        public Guid TouristId { get; set; }
        public List<Guid> TourIds { get; set; }
        public decimal BonusPointsToUse { get; set; }
    }

    public class PurchaseTourResult
    {
        public bool Success { get; set; }
        public Guid PurchaseId { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal BonusPointsUsed { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
