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
    public class MonthlyReportRepository : RepositoryBase<MonthlyReport>, IMonthlyReportRepository
    {
        public MonthlyReportRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<MonthlyReport> GetByGuideAndMonthAsync(Guid guideId, int month, int year)
        {
            return await _context.MonthlyReports
                .Include(r => r.TourSales)
                .FirstOrDefaultAsync(r => r.GuideId == guideId && r.Month == month && r.Year == year);
        }

        public async Task<bool> ExistsAsync(Guid guideId, int month, int year)
        {
            return await _context.MonthlyReports
                .AnyAsync(r => r.GuideId == guideId && r.Month == month && r.Year == year);
        }
    }
}
