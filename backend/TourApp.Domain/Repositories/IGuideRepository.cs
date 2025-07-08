using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;

namespace TourApp.Domain.Repositories
{
    public interface IGuideRepository : IRepository<Guide>
    {
        Task<Guide> GetWithToursAsync(Guid id);
        Task<IEnumerable<Guide>> GetRewardedGuidesAsync();
        Task<IEnumerable<Guide>> GetMaliciousGuidesAsync();
        Task<Guide> GetTopGuideForMonthAsync(int month, int year);
    }
}
