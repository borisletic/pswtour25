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
    public class ProblemRepository : RepositoryBase<Problem>, IProblemRepository
    {
        public ProblemRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<Problem> GetWithEventsAsync(Guid id)
        {
            return await _context.Problems
                .Include(p => p.Events)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Problem>> GetByTouristAsync(Guid touristId)
        {
            return await _context.Problems
                .Where(p => p.TouristId == touristId)
                .Include(p => p.Events)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Problem>> GetByTourAsync(Guid tourId)
        {
            return await _context.Problems
                .Where(p => p.TourId == tourId)
                .Include(p => p.Events)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Problem>> GetByStatusAsync(ProblemStatus status)
        {
            return await _context.Problems
                .Where(p => p.Status == status)
                .Include(p => p.Events)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetRejectedCountByTouristAsync(Guid touristId)
        {
            return await _context.Problems
                .CountAsync(p => p.TouristId == touristId && p.Status == ProblemStatus.Rejected);
        }
    }
}
