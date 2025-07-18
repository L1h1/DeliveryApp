﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.DAL.Data;
using UserService.DAL.Interfaces.Repositories;
using UserService.DAL.Models;
using UserService.DAL.Options;
using UserService.DAL.Repositories;

namespace UserService.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnection")));
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(
                configuration.GetSection(nameof(JwtOptions)));

            services.Configure<EmailOptions>(
                configuration.GetSection(nameof(EmailOptions)));

            services.Configure<RabbitMqOptions>(
                configuration.GetSection(nameof(RabbitMqOptions)));

            return services;
        }

        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = configuration.GetConnectionString("Redis");
                opt.InstanceName = "Users_";
            });

            return services;
        }
    }
}
