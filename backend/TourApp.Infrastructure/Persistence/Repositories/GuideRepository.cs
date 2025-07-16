using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Infrastructure.Persistence.Context;

namespace TourApp.Infrastructure.Persistence.Repositories
{
    public class GuideRepository : RepositoryBase<Guide>, IGuideRepository
    {
        public GuideRepository(TourAppDbContext context) : base(context)
        {
        }

        public async Task<Guide> GetWithToursAsync(Guid id)
        {
            return await _context.Guides
                .Include(g => g.Tours)
                    .ThenInclude(t => t.KeyPoints)
                .Include(g => g.Tours)
                    .ThenInclude(t => t.Ratings)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Guide> GetByIdAsync(Guid id)
        {
            return await _context.Guides.FindAsync(id);
        }

        public async Task<IEnumerable<Guide>> GetRewardedGuidesAsync()
        {
            return await _context.Guides
                .Where(g => g.RewardPoints >= 5)
                .ToListAsync();
        }

        public async Task<IEnumerable<Guide>> GetMaliciousGuidesAsync()
        {
            return await _context.Guides
                .Where(g => g.CancelledToursCount >= 10)
                .ToListAsync();
        }

        public async Task<Guide> GetTopGuideForMonthAsync(int month, int year)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var guideSales = await _context.Purchases
                .Where(p => p.PurchasedAt >= startDate && p.PurchasedAt <= endDate)
                .SelectMany(p => p.Tours)
                .GroupBy(t => t.GuideId)
                .Select(g => new { GuideId = g.Key, SalesCount = g.Count() })
                .OrderByDescending(g => g.SalesCount)
                .FirstOrDefaultAsync();

            if (guideSales != null)
            {
                return await GetByIdAsync(guideSales.GuideId);
            }

            return null;
        }

        public async Task<Guide> GetTopGuideBySalesAsync(DateTime monthStart, DateTime monthEnd)
        {
            var guideSales = await GetGuideSalesCountAsync(monthStart, monthEnd);

            if (!guideSales.Any())
                return null;

            var topGuideId = guideSales.OrderByDescending(kvp => kvp.Value).First().Key;
            return await GetByIdAsync(topGuideId);
        }

        public async Task<Dictionary<Guid, int>> GetGuideSalesCountAsync(DateTime monthStart, DateTime monthEnd)
        {
            var results = new Dictionary<Guid, int>();

            try
            {
                // Pokušaj kroz navigation properties
                var purchases = await _context.Purchases
                    .Where(p => p.PurchasedAt >= monthStart && p.PurchasedAt <= monthEnd)
                    .Include(p => p.Tours)
                    .ToListAsync();

                foreach (var purchase in purchases)
                {
                    if (purchase.Tours != null)
                    {
                        foreach (var tour in purchase.Tours)
                        {
                            if (!results.ContainsKey(tour.GuideId))
                                results[tour.GuideId] = 0;

                            results[tour.GuideId]++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback na raw SQL ako navigation ne radi
                Console.WriteLine($"Navigation properties error: {ex.Message}. Using SQL fallback.");
                return await GetGuideSalesCountViaSqlAsync(monthStart, monthEnd);
            }

            return results;
        }

        private async Task<Dictionary<Guid, int>> GetGuideSalesCountViaSqlAsync(DateTime monthStart, DateTime monthEnd)
        {
            var sql = @"
                SELECT t.GuideId, COUNT(*) as SalesCount
                FROM Purchases p
                INNER JOIN PurchaseTour pt ON p.Id = pt.PurchasesId
                INNER JOIN Tours t ON pt.ToursId = t.Id
                WHERE p.PurchasedAt >= @monthStart AND p.PurchasedAt <= @monthEnd
                GROUP BY t.GuideId";

            var connection = _context.Database.GetDbConnection();
            var results = new Dictionary<Guid, int>();

            try
            {
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = sql;

                var startParam = command.CreateParameter();
                startParam.ParameterName = "@monthStart";
                startParam.Value = monthStart;
                command.Parameters.Add(startParam);

                var endParam = command.CreateParameter();
                endParam.ParameterName = "@monthEnd";
                endParam.Value = monthEnd;
                command.Parameters.Add(endParam);

                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var guideId = reader.GetGuid("GuideId");
                    var salesCount = reader.GetInt32("SalesCount");
                    results[guideId] = salesCount;
                }
            }
            finally
            {
                await connection.CloseAsync();
            }

            return results;
        }

    }
}
