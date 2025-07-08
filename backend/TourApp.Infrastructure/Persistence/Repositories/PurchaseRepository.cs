using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Domain.ValueObjects;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.Infrastructure.Persistence.Repositories
{
    public class PurchaseRepository : RepositoryBase<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Purchase>> GetByTouristAsync(Guid touristId)
        {
            return await _context.Purchases
                .Where(p => p.TouristId == touristId)
                .Include(p => p.Tours)
                    .ThenInclude(t => t.KeyPoints)
                .Include(p => p.Tours)
                    .ThenInclude(t => t.GuideId)
                .OrderByDescending(p => p.PurchasedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Purchase>> GetByTourAsync(Guid tourId)
        {
            return await _context.Purchases
                .Where(p => p.Tours.Any(t => t.Id == tourId))
                .Include(p => p.Tours)
                .ToListAsync();
        }

        public async Task<bool> HasTouristPurchasedTourAsync(Guid touristId, Guid tourId)
        {
            return await _context.Purchases
                .AnyAsync(p => p.TouristId == touristId && p.Tours.Any(t => t.Id == tourId));
        }

        public async Task<IEnumerable<Purchase>> GetPurchasesForReminderAsync(DateTime reminderDate)
        {
            var reminderDateStart = reminderDate.Date;
            var reminderDateEnd = reminderDateStart.AddDays(1).AddSeconds(-1);

            return await _context.Purchases
                .Include(p => p.Tours)
                    .ThenInclude(t => t.KeyPoints)
                .Where(p => p.Tours.Any(t =>
                    t.ScheduledDate >= reminderDateStart &&
                    t.ScheduledDate <= reminderDateEnd &&
                    t.Status == TourStatus.Published))
                .ToListAsync();
        }
    }
}
