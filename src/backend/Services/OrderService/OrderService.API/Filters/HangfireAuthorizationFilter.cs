using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace OrderService.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return context.GetHttpContext().User.IsInRole("Admin");
        }
    }
}
