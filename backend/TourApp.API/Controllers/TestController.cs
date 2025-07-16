using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Services;
using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IGuideRepository _guideRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IMonthlyReportRepository _monthlyReportRepository;
    private readonly IEmailService _emailService;

    public TestController(
        INotificationService notificationService,
        IGuideRepository guideRepository,
        ITourRepository tourRepository,
        IMonthlyReportRepository monthlyReportRepository,
        IEmailService emailService)
    {
        _notificationService = notificationService;
        _guideRepository = guideRepository;
        _tourRepository = tourRepository;
        _monthlyReportRepository = monthlyReportRepository;
        _emailService = emailService;
    }

    [HttpPost("send-monthly-reports")]
    public async Task<IActionResult> TestMonthlyReports()
    {
        try
        {
            await _notificationService.SendMonthlyReportsAsync();
            return Ok("Monthly reports sent successfully");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    // NOVA METODA: Testiranje sa trenutnim mesecom
    [HttpPost("send-monthly-reports-current")]
    public async Task<IActionResult> TestMonthlyReportsCurrentMonth()
    {
        try
        {
            var now = DateTime.UtcNow;
            var currentMonth = now.Month;
            var currentYear = now.Year;

            var allGuides = await _guideRepository.GetAllAsync();

            foreach (var guide in allGuides.Where(g => !g.IsBlocked))
            {
                // Skip if report already exists
                if (await _monthlyReportRepository.ExistsAsync(guide.Id, currentMonth, currentYear))
                    continue;

                // Get guide's tours
                var guideTours = await _tourRepository.GetByGuideAsync(guide.Id);
                if (!guideTours.Any())
                    continue;

                // Create monthly report for CURRENT month
                var report = new MonthlyReport(guide.Id, currentMonth, currentYear);

                // Calculate sales for current month
                var startDate = new DateTime(currentYear, currentMonth, 1);
                var endDate = startDate.AddMonths(1).AddSeconds(-1);

                var reportData = new MonthlyReportData
                {
                    GuideName = $"{guide.FirstName} {guide.LastName}",
                    Month = currentMonth,
                    Year = currentYear,
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

            return Ok("Monthly reports for current month sent successfully");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    // NOVA METODA: Testiranje sa test podacima
    [HttpPost("send-test-monthly-report")]
    public async Task<IActionResult> SendTestMonthlyReport([FromBody] TestReportRequest request)
    {
        try
        {
            // Kreiraj test podatke
            var testReportData = new MonthlyReportData
            {
                GuideName = request.GuideName ?? "Test Vodič",
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                TourSales = new List<TourSalesDto>
                {
                    new TourSalesDto { TourName = "Test Tura 1", SalesCount = 5 },
                    new TourSalesDto { TourName = "Test Tura 2", SalesCount = 3 },
                    new TourSalesDto { TourName = "Test Tura 3", SalesCount = 8 }
                },
                TotalSales = 16,
                BestRatedTourName = "Test Tura 3",
                BestRatedScore = 4.8,
                BestRatedCount = 12,
                WorstRatedTourName = "Test Tura 1",
                WorstRatedScore = 3.2,
                WorstRatedCount = 8
            };

            // Pošalji email na test adresu
            await _emailService.SendMonthlyReportAsync(request.Email, testReportData);

            return Ok($"Test monthly report sent to {request.Email}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}

public class TestReportRequest
{
    public string Email { get; set; } = "";
    public string? GuideName { get; set; }
}

/*fetch('https://localhost:7232/api/test/send-test-monthly-report', {
method: 'POST',
  headers:
{
    'Authorization': 'Bearer ' + localStorage.getItem('auth_token'),
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
email: 'tvoj-email@example.com',  // Zameni sa svojim email-om
    guideName: 'Marko Marković'
  })
})
.then(response => response.text())
.then(data => console.log(data))
.catch(error => console.error('Error:', error));
*/