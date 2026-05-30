using Hangfire.Dashboard;

namespace AeroManage.API.Middleware
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In production, implement proper authentication
            // For development, allow all
            return true; // TODO: Implement proper authorization
        }
    }
}
