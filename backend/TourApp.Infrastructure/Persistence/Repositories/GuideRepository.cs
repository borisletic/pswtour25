using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.Infrastructure.Persistence.Repositories
{
    public class GuideRepository : RepositoryBase<Guide>, IGuideRepository
    {
        public GuideRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<Guide> GetWithToursAsync(Guid id)
        {
            return await _context.Guides
                .Include(g => g.Tours)
                    .ThenInclude(t => t.KeyPoints)
                .Include(g => g.Tours)
                    .ThenInclude(t => t.Ratings)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Guide>> GetRewardedGuidesAsync()
        {
            return await _context.Guides
                .Where(g => g.RewardPoints >= 5)
                .ToListAsync();
        }

        public async Task<IEnumerable<Guide>> GetMaliciousGuidesAsync()
        {
            return await _context.Guides
                .Where(g => g.CancelledToursCount >= 10)
                .ToListAsync();
        }

        public async Task<Guide> GetTopGuideForMonthAsync(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var guideSales = await _context.Purchases
                .Where(p => p.PurchasedAt >= startDate && p.PurchasedAt <= endDate)
                .SelectMany(p => p.Tours)
                .GroupBy(t => t.GuideId)
                .Select(g => new { GuideId = g.Key, SalesCount = g.Count() })
                .OrderByDescending(g => g.SalesCount)
                .FirstOrDefaultAsync();

            if (guideSales != null)
            {
                return await GetByIdAsync(guideSales.GuideId);
            }

            return null;
        }
    }
}
