using Hangfire;
using JwtAuthentication;
using UserService.API;
using UserService.API.Filters;
using UserService.API.Middleware;
using UserService.BLL;
using UserService.DAL;
using UserService.DAL.Data;
using UserService.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilogLogging();

// Add services to the container.
builder.Services
    .AddBLL()
    .AddDataAccess(builder.Configuration)
    .AddOptions(builder.Configuration)
    .AddIdentity()
    .AddJwtAuthentication(builder.Configuration)
    .AddRabbitMq()
    .AddRedisCaching(builder.Configuration);

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHangfireScheduler(builder.Configuration);
}

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

if (app.Environment.IsEnvironment("Testing"))
{
    await DbInitializer.SeedData(app.Services);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHangfireDashboard("/jobs", new DashboardOptions()
    {
        Authorization = new[] { new HangfireAuthorizationFilter() },
    });
}

app.MapControllers();

app.Run();

public partial class Program;