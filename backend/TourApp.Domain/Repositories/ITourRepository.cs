using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;

namespace TourApp.Domain.Repositories
{
    public interface ITourRepository : IRepository<Tour>
    {
        Task<Tour> GetWithKeyPointsAsync(Guid id);
        Task<Tour> GetWithRatingsAsync(Guid id);
        Task<IEnumerable<Tour>> GetPublishedToursAsync();
        Task<IEnumerable<Tour>> GetByGuideAsync(Guid guideId);
        Task<IEnumerable<Tour>> GetByCategoryAsync(Interest category);
        Task<IEnumerable<Tour>> GetUpcomingToursAsync();
        Task<int> GetSoldCountAsync(Guid tourId, DateTime startDate, DateTime endDate);
    }
}
