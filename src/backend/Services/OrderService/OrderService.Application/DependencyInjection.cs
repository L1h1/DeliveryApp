using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Protos;
using OrderService.Application.Services;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace OrderService.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddAutoMapper(assembly);

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(assembly));

            services.AddValidatorsFromAssembly(assembly);
            services.AddFluentValidationAutoValidation();

            services.AddScoped<IBillService, PdfBillService>();
        }

        public static void AddGrpc(this IServiceCollection services, IConfiguration configuration)
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
        }
    }
}
