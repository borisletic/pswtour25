using TourApp.Domain.Services;

namespace TourApp.Application.BackgroundJobs
{
    public class MonthlyGuideRewardJob
    {
        private readonly IGuideRewardService _guideRewardService;

        public MonthlyGuideRewardJob(IGuideRewardService guideRewardService)
        {
            _guideRewardService = guideRewardService;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                var lastMonth = DateTime.Now.AddMonths(-1);
                var topGuide = await _guideRewardService.GetTopGuideBySalesAsync(lastMonth);

                if (topGuide != null)
                {
                    var oldPoints = topGuide.RewardPoints;
                    await _guideRewardService.AwardGuideAsync(topGuide.Id);

                    Console.WriteLine($"Mesečno nagrađivanje: Vodič {topGuide.FirstName} {topGuide.LastName} nagrađen! Bodovi: {oldPoints} → {oldPoints + 1}");

                    if (oldPoints + 1 == 5)
                    {
                        Console.WriteLine($"🎉 Vodič {topGuide.FirstName} {topGuide.LastName} je sada NAGRAĐIVANI VODIČ! (5+ bodova)");
                    }
                }
                else
                {
                    Console.WriteLine($"Mesečno nagrađivanje: Nema vodiča za nagrađivanje za {lastMonth:yyyy-MM}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Greška u mesečnom nagrađivanju vodiča: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}