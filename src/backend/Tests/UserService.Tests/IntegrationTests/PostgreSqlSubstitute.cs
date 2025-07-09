using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using UserService.DAL.Data;
using UserService.DAL.Models;

namespace UserService.Tests.IntegrationTests
{
    public class PostgreSqlSubstitute : IAsyncLifetime
    {
        private PostgreSqlContainer _dbContainer;
        public UserManager<User> UserManager { get; private set; }
        public RoleManager<IdentityRole<Guid>> RoleManager { get; private set; }
        public AppDbContext DbContext { get; private set; }

        public async Task DisposeAsync()
        {
            await DbContext.Database.EnsureDeletedAsync();
            await _dbContainer.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithDatabase("users")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _dbContainer.StartAsync();

            var services = new ServiceCollection();

            services.AddLogging();
            services.AddHttpContextAccessor();

            var connStr = _dbContainer.GetConnectionString();
            services.AddDbContext<AppDbContext>(
                options => options.UseNpgsql(connStr));

            services.AddIdentity<User, IdentityRole<Guid>>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var provider = services.BuildServiceProvider();
            DbContext = provider.GetRequiredService<AppDbContext>();
            await DbContext.Database.MigrateAsync();

            UserManager = provider.GetRequiredService<UserManager<User>>();
            RoleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            await SeedData();
        }

        private async Task SeedData()
        {
            string[] roles = { "Admin", "Client", "Courier" };
            foreach (var role in roles)
            {
                await RoleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            var user = new User()
            {
                UserName ="test@test.com",
                Email = "test@test.com"
            };
            var passwd = "Password123!";

            await UserManager.CreateAsync(user, passwd);
            await UserManager.AddToRoleAsync(user, "Admin");
        }
    }
}
