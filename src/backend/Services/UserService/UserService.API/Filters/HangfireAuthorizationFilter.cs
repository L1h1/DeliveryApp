using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace UserService.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return context.GetHttpContext().User.IsInRole("Admin");
        }
    }
}
