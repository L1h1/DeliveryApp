using Hangfire;
using OrderService.API;
using OrderService.API.Filters;
using OrderService.API.Middleware;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilogLogging();

// Add services to the container.
builder.Services
    .AddApplication()
    .AddOptions(builder.Configuration)
    .AddGrpc(builder.Configuration)
    .AddDataAccess(builder.Configuration)
    .AddBackgroundJobs(builder.Configuration)
    .AddRabbitMq();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.RunRecurringJobs();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/jobs", new DashboardOptions()
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
});

app.MapControllers();

app.Run();
