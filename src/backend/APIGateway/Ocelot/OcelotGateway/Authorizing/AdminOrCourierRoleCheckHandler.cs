using System.Net;

namespace OcelotGateway.Authorizing
{
    public class AdminOrCourierRoleCheckHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminOrCourierRoleCheckHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if (!user.IsInRole("Admin") && !user.IsInRole("Courier"))
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
