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
    public class TouristRepository : RepositoryBase<Tourist>, ITouristRepository
    {
        public TouristRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<Tourist> GetWithPurchasesAsync(Guid id)
        {
            return await _context.Tourists
                .Include(t => t.Purchases)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tourist>> GetByInterestsAsync(List<Interest> interests)
        {
            Console.WriteLine($"🔍 Searching for tourists with interests: {string.Join(", ", interests)}");

            // Za sada, jednostavnije rešenje - učitaj sve turiste pa filtriraj u memoriji
            var allTourists = await _context.Tourists
                .Where(t => !t.IsBlocked)  // Dodano da se odma filtriraju nelokirani
                .ToListAsync();

            Console.WriteLine($"🔍 Loaded {allTourists.Count} tourists from database");

            // Filtriraj u memoriji
            var matchingTourists = allTourists
                .Where(t => t.Interests.Any(i => interests.Contains(i)))
                .ToList();

            Console.WriteLine($"🔍 Found {matchingTourists.Count} tourists with matching interests");

            foreach (var tourist in matchingTourists)
            {
                Console.WriteLine($"🔍 - {tourist.FirstName} {tourist.LastName} ({tourist.Email}) - Interests: {string.Join(", ", tourist.Interests)}");
            }

            return matchingTourists;
        }

        public async Task<IEnumerable<Tourist>> GetMaliciousTouristsAsync()
        {
            return await _context.Tourists
                .Where(t => t.InvalidProblemsCount >= 10)
                .ToListAsync();
        }
    }
}
