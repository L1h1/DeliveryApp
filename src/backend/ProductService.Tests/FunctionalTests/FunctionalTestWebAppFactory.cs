using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using UserService.DAL.Data;
using Moq;
using UserService.BLL.Interfaces;
using System.Linq.Expressions;
using Hangfire;
using Microsoft.Extensions.Hosting;

namespace UserService.Tests.FunctionalTests
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithDatabase("users")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        private RedisContainer _redisContainer = new RedisBuilder()
                .WithImage("redis:latest")
                .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll<IHostedService>();

                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseNpgsql(_dbContainer.GetConnectionString()));

                var backgroundJobServiceStub = new Mock<IBackgroundJobService>();
                backgroundJobServiceStub.Setup(m => m.CreateJob(It.IsAny<Expression<Action>>()));

                services.AddSingleton(backgroundJobServiceStub.Object);
            });
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            await _redisContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();
        }
    }
}
