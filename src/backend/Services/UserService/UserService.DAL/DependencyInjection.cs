using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL.Data;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;
using UserService.DAL.Repositories;


namespace UserService.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConntection")));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddIdentity<User, IdentityRole<Guid>>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;
                opt.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
