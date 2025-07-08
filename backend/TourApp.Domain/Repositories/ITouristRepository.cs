using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Enums;

namespace TourApp.Domain.Repositories
{
    public interface ITouristRepository : IRepository<Tourist>
    {
        Task<Tourist> GetWithPurchasesAsync(Guid id);
        Task<IEnumerable<Tourist>> GetByInterestsAsync(List<Interest> interests);
        Task<IEnumerable<Tourist>> GetMaliciousTouristsAsync();
    }
}
