using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Options;
using OrderService.Application.Protos;
using OrderService.Application.Services;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace OrderService.Application
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

            return services;
        }

        public static IServiceCollection ConfigurePDF(this IServiceCollection services)
        {
            GlobalFontSettings.FontResolver = new FailsafeFontResolver();

            services.AddScoped<IPDFService, BillPDFService>();

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageOptions>(
                configuration.GetSection(nameof(StorageOptions)));

            return services;
        }

        public static IServiceCollection AddGrpc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddGrpcClient<UserService.UserServiceClient>(cfg =>
            {
                cfg.Address = new Uri(configuration["GrpcUserServiceUrl"]);
            });
            services.AddScoped<IUserService, GrpcUserService>();

            services.AddGrpcClient<ProductService.ProductServiceClient>(cfg =>
            {
                cfg.Address = new Uri(configuration["GrpcProductServiceUrl"]);
            });
            services.AddScoped<IProductService, GrpcProductService>();

            return services;
        }
    }
}
