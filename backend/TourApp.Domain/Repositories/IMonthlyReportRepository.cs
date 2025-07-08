using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;

namespace TourApp.Domain.Repositories
{
    public interface IMonthlyReportRepository : IRepository<MonthlyReport>
    {
        Task<MonthlyReport> GetByGuideAndMonthAsync(Guid guideId, int month, int year);
        Task<bool> ExistsAsync(Guid guideId, int month, int year);
    }
}
