using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Data.Repositories;

namespace OrderService.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));

            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton<MongoDbContext>();

            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
