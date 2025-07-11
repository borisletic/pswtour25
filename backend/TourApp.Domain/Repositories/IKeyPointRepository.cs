using TourApp.Domain.Entities;

namespace TourApp.Domain.Repositories
{
    public interface IKeyPointRepository : IRepository<KeyPoint>
    {
        Task<IEnumerable<KeyPoint>> GetByTourIdAsync(Guid tourId);
    }
}