using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderService.Application.Interfaces.Messaging.Producers;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Data.Repositories;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Messaging.Producers;
using OrderService.Infrastructure.Services;

namespace OrderService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDBConnection"));

            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton<MongoDbContext>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }

        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
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

        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<RabbitMqConnection>();
            services.AddScoped<IMessageProducer, RabbitMqProducer>();

            return services;
        }
    }
}
