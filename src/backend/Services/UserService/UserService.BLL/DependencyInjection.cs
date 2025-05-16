using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UserService.BLL.Interfaces;
using UserService.BLL.Services;

namespace UserService.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
