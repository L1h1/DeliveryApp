using Hangfire;
using OrderService.API.Filters;
using OrderService.API.Middleware;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddGrpc(builder.Configuration);

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
