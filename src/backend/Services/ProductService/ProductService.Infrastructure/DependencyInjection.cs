using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Infrastructure.Data.Caching;
using ProductService.Infrastructure.Data.NoSQL;
using ProductService.Infrastructure.Data.NoSQL.Repositories;
using ProductService.Infrastructure.Data.SQL;
using ProductService.Infrastructure.Data.SQL.Repositories;

namespace ProductService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));

            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton<MongoDbContext>();

            services.AddDbContext<EFDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnection"));
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IProductDetailsRepository, ProductDetailsRepository>();

            return services;
        }

        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("Redis");
                opt.InstanceName = "Products_";
            });

            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
