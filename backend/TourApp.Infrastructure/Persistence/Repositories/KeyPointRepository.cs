using Microsoft.EntityFrameworkCore;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.Infrastructure.Persistence.Repositories
{
    public class KeyPointRepository : RepositoryBase<KeyPoint>, IKeyPointRepository
    {
        public KeyPointRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<KeyPoint>> GetByTourIdAsync(Guid tourId)
        {
            return await _context.KeyPoints
                .Where(kp => kp.TourId == tourId)
                .OrderBy(kp => kp.Order)
                .ToListAsync();
        }
    }
}