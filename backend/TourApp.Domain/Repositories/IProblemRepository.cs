using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;

namespace TourApp.Domain.Repositories
{
    public interface IProblemRepository : IRepository<Problem>
    {
        Task<Problem> GetWithEventsAsync(Guid id);
        Task<IEnumerable<Problem>> GetByTouristAsync(Guid touristId);
        Task<IEnumerable<Problem>> GetByTourAsync(Guid tourId);
        Task<IEnumerable<Problem>> GetByStatusAsync(ProblemStatus status);
        Task<int> GetRejectedCountByTouristAsync(Guid touristId);
    }
}
