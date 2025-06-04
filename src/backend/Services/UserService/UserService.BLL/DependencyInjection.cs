using System.Reflection;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using UserService.BLL.Interfaces;
using UserService.BLL.Services;

namespace UserService.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();

            return services;
        }

        public static IServiceCollection AddHangfireScheduler(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(cfg =>
            {
                cfg.UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(c =>
                        c.UseNpgsqlConnection(configuration.GetConnectionString("PostgreSQLConnection")));
            });

            services.AddHangfireServer();

            services.AddScoped<IBackgroundJobService, BackgroundJobService>();

            return services;
        }
    }
}
