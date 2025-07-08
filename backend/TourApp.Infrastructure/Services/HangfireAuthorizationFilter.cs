using Hangfire.Annotations;
using Hangfire.Dashboard;



namespace TouristTours.Infrastructure.Services
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Allow all authenticated users with Administrator role
            return httpContext.User.Identity.IsAuthenticated &&
                   httpContext.User.IsInRole("Administrator");
        }
    }
}