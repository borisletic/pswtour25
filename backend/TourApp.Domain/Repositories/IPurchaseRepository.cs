using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;

namespace TourApp.Domain.Repositories
{
    public interface IPurchaseRepository : IRepository<Purchase>
    {
        Task<IEnumerable<Purchase>> GetByTouristAsync(Guid touristId);
        Task<IEnumerable<Purchase>> GetByTourAsync(Guid tourId);
        Task<bool> HasTouristPurchasedTourAsync(Guid touristId, Guid tourId);
        Task<IEnumerable<Purchase>> GetPurchasesForReminderAsync(DateTime reminderDate);
    }
}
