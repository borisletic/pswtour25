using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Domain.ValueObjects;

namespace TourApp.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ITouristRepository _touristRepository;
        private readonly IGuideRepository _guideRepository;
        private readonly IMonthlyReportRepository _monthlyReportRepository;
        private readonly IEmailService _emailService;

        public NotificationService(
            ITourRepository tourRepository,
            IPurchaseRepository purchaseRepository,
            ITouristRepository touristRepository,
            IGuideRepository guideRepository,
            IMonthlyReportRepository monthlyReportRepository,
            IEmailService emailService)
        {
            _tourRepository = tourRepository;
            _purchaseRepository = purchaseRepository;
            _touristRepository = touristRepository;
            _guideRepository = guideRepository;
            _monthlyReportRepository = monthlyReportRepository;
            _emailService = emailService;
        }

        public async Task SendTourRemindersAsync()
        {
            // Get tours scheduled for 48 hours from now
            var reminderDate = DateTime.UtcNow.AddHours(48);
            var reminderDateStart = reminderDate.Date;
            var reminderDateEnd = reminderDateStart.AddDays(1).AddSeconds(-1);

            var upcomingTours = await _tourRepository.GetUpcomingToursAsync();
            var toursToRemind = upcomingTours.Where(t =>
                t.ScheduledDate >= reminderDateStart &&
                t.ScheduledDate <= reminderDateEnd &&
                t.Status == TourStatus.Published);

            foreach (var tour in toursToRemind)
            {
                // Get all purchases for this tour
                var purchases = await _purchaseRepository.GetByTourAsync(tour.Id);

                foreach (var purchase in purchases)
                {
                    var tourist = await _touristRepository.GetByIdAsync(purchase.TouristId);
                    if (tourist != null && !tourist.IsBlocked)
                    {
                        var keyPoint = tour.KeyPoints.FirstOrDefault();
                        await _emailService.SendTourReminderAsync(tourist.Email, new TourReminderData
                        {
                            TourName = tour.Name,
                            ScheduledDate = tour.ScheduledDate,
                            MeetingPoint = keyPoint?.Name ?? "To be announced"
                        });
                    }
                }
            }
        }

        public async Task SendMonthlyReportsAsync()
        {
            var now = DateTime.UtcNow;
            var lastMonth = now.AddMonths(-1);
            var month = lastMonth.Month;
            var year = lastMonth.Year;

            var allGuides = await _guideRepository.GetAllAsync();

            foreach (var guide in allGuides.Where(g => !g.IsBlocked))
            {
                // Check if report already exists
                if (await _monthlyReportRepository.ExistsAsync(guide.Id, month, year))
                    continue;

                // Get guide's tours
                var guideTours = await _tourRepository.GetByGuideAsync(guide.Id);
                if (!guideTours.Any())
                    continue;

                // Create monthly report
                var report = new MonthlyReport(guide.Id, month, year);

                // Calculate sales for each tour
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddSeconds(-1);

                var reportData = new MonthlyReportData
                {
                    GuideName = $"{guide.FirstName} {guide.LastName}",
                    Month = month,
                    Year = year,
                    TourSales = new List<TourSalesDto>()
                };

                foreach (var tour in guideTours)
                {
                    var salesCount = await _tourRepository.GetSoldCountAsync(tour.Id, startDate, endDate);
                    if (salesCount > 0)
                    {
                        report.AddTourSales(tour.Id, tour.Name, salesCount);
                        reportData.TourSales.Add(new TourSalesDto
                        {
                            TourName = tour.Name,
                            SalesCount = salesCount
                        });
                    }
                }

                reportData.TotalSales = report.GetTotalSales();

                // Find best and worst rated tours
                var toursWithRatings = guideTours.Where(t => t.Ratings.Any()).ToList();

                if (toursWithRatings.Any())
                {
                    var bestRated = toursWithRatings.OrderByDescending(t => t.GetAverageRating()).First();
                    var worstRated = toursWithRatings.OrderBy(t => t.GetAverageRating()).First();

                    report.SetBestRatedTour(bestRated.Id, bestRated.GetAverageRating(), bestRated.Ratings.Count);
                    report.SetWorstRatedTour(worstRated.Id, worstRated.GetAverageRating(), worstRated.Ratings.Count);

                    reportData.BestRatedTourName = bestRated.Name;
                    reportData.BestRatedScore = bestRated.GetAverageRating();
                    reportData.BestRatedCount = bestRated.Ratings.Count;
                    reportData.WorstRatedTourName = worstRated.Name;
                    reportData.WorstRatedScore = worstRated.GetAverageRating();
                    reportData.WorstRatedCount = worstRated.Ratings.Count;
                }

                // Save report
                await _monthlyReportRepository.AddAsync(report);
                await _monthlyReportRepository.SaveChangesAsync();

                // Send email
                await _emailService.SendMonthlyReportAsync(guide.Email, reportData);
            }
        }

        public async Task ProcessMonthlyRewardsAsync()
        {
            var now = DateTime.UtcNow;
            var lastMonth = now.AddMonths(-1);
            var month = lastMonth.Month;
            var year = lastMonth.Year;

            // Find guide with most sales
            var topGuide = await _guideRepository.GetTopGuideForMonthAsync(month, year);

            if (topGuide != null)
            {
                topGuide.AddRewardPoint();
                _guideRepository.Update(topGuide);
                await _guideRepository.SaveChangesAsync();
            }
        }
    }
}