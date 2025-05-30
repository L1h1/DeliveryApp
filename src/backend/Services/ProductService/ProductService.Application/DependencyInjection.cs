using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Application.Interfaces.Services;
using ProductService.Application.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace ProductService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddAutoMapper(assembly);

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(assembly));

            services.AddValidatorsFromAssembly(assembly);
            services.AddFluentValidationAutoValidation();

            services.AddScoped<IImageStorageService, ImageStorageService>();

            return services;
        }
    }
}
