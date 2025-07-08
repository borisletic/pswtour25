using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Application.DTOs
{
    public class PurchaseHistoryDto
    {
        public Guid Id { get; set; }
        public DateTime PurchasedAt { get; set; }
        public List<TourDto> Tours { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal BonusPointsUsed { get; set; }
        public string Currency { get; set; }
    }
}
