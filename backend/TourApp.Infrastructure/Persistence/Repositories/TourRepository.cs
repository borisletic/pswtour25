using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;
using TourApp.Domain.Repositories;
using TourApp.Domain.ValueObjects;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.Infrastructure.Persistence.Repositories
{
    public class TourRepository : RepositoryBase<Tour>, ITourRepository
    {
        public TourRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<Tour> GetWithKeyPointsAsync(Guid id)
        {
            return await _context.Tours
                .Include(t => t.KeyPoints)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour> GetWithRatingsAsync(Guid id)
        {
            return await _context.Tours
                .Include(t => t.Ratings)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tour>> GetPublishedToursAsync()
        {
            return await _context.Tours
                .Where(t => t.Status == TourStatus.Published)
                .Include(t => t.KeyPoints)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetByGuideAsync(Guid guideId)
        {
            return await _context.Tours
                .Where(t => t.GuideId == guideId)
                .Include(t => t.KeyPoints)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetByCategoryAsync(Interest category)
        {
            return await _context.Tours
                .Where(t => t.Category == category && t.Status == TourStatus.Published)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetUpcomingToursAsync()
        {
            return await _context.Tours
                .Where(t => t.Status == TourStatus.Published && t.ScheduledDate > DateTime.UtcNow)
                .OrderBy(t => t.ScheduledDate)
                .ToListAsync();
        }

        public async Task<int> GetSoldCountAsync(Guid tourId, DateTime startDate, DateTime endDate)
        {
            return await _context.Purchases
                .Where(p => p.PurchasedAt >= startDate && p.PurchasedAt <= endDate)
                .Where(p => p.Tours.Any(t => t.Id == tourId))
                .CountAsync();
        }
    }
}
