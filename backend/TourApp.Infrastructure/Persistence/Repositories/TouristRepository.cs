using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;
using TourApp.Domain.Repositories;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.Infrastructure.Persistence.Repositories
{
    public class TouristRepository : RepositoryBase<Tourist>, ITouristRepository
    {
        public TouristRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<Tourist> GetWithPurchasesAsync(Guid id)
        {
            return await _context.Tourists
                .Include(t => t.Purchases)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tourist>> GetByInterestsAsync(List<Interest> interests)
        {
            return await _context.Tourists
                .Where(t => t.Interests.Any(i => interests.Contains(i)))
                .ToListAsync();
        }

        public async Task<IEnumerable<Tourist>> GetMaliciousTouristsAsync()
        {
            return await _context.Tourists
                .Where(t => t.InvalidProblemsCount >= 10)
                .ToListAsync();
        }
    }
}
