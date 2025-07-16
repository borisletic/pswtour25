using TourApp.Domain.Entities;

namespace TourApp.Domain.Services
{
    public interface IGuideRewardService
    {
        Task<Guide> GetTopGuideBySalesAsync(DateTime month);
        Task<bool> IsRewardedGuideAsync(Guid guideId);
        Task<IEnumerable<Guide>> GetRewardedGuidesAsync();
        Task AwardGuideAsync(Guid guideId);
    }
}