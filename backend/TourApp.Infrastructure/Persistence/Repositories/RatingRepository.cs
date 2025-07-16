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
    public class RatingRepository : RepositoryBase<Rating>, IRatingRepository
    {
        public RatingRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<Rating> GetByTouristAndTourAsync(Guid touristId, Guid tourId)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(r => r.TouristId == touristId && r.TourId == tourId);
        }

        public async Task<bool> ExistsAsync(Guid touristId, Guid tourId)
        {
            return await _context.Ratings
                .AnyAsync(r => r.TouristId == touristId && r.TourId == tourId);
        }

        
    }
}
