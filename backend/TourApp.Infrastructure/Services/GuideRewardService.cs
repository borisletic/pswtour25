using TourApp.Domain.Entities;
using TourApp.Domain.Repositories;
using TourApp.Domain.Services;

namespace TourApp.Infrastructure.Services
{
    public class GuideRewardService : IGuideRewardService
    {
        private readonly IGuideRepository _guideRepository;

        public GuideRewardService(IGuideRepository guideRepository)
        {
            _guideRepository = guideRepository;
        }

        public async Task<Guide> GetTopGuideBySalesAsync(DateTime month)
        {
            var monthStart = new DateTime(month.Year, month.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            return await _guideRepository.GetTopGuideBySalesAsync(monthStart, monthEnd);
        }

        public async Task<bool> IsRewardedGuideAsync(Guid guideId)
        {
            var guide = await _guideRepository.GetByIdAsync(guideId);
            return guide != null && guide.RewardPoints >= 5;
        }

        public async Task<IEnumerable<Guide>> GetRewardedGuidesAsync()
        {
            return await _guideRepository.GetRewardedGuidesAsync();
        }

        public async Task AwardGuideAsync(Guid guideId)
        {
            var guide = await _guideRepository.GetByIdAsync(guideId);
            if (guide != null)
            {
                guide.RewardPoints += 1;
                _guideRepository.Update(guide);
                await _guideRepository.SaveChangesAsync();
            }
        }
    }
}