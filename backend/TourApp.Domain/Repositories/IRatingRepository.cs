using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;

namespace TourApp.Domain.Repositories
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task<Rating> GetByTouristAndTourAsync(Guid touristId, Guid tourId);
        Task<bool> ExistsAsync(Guid touristId, Guid tourId);
    }
}
