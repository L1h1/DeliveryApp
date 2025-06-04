using Hangfire;
using UserService.API;
using UserService.API.Filters;
using UserService.API.Middleware;
using UserService.BLL;
using UserService.DAL;
using UserService.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddData(builder.Configuration);
builder.Services.AddJWTAuth(builder.Configuration);
builder.Services.AddBLL();
builder.Services.AddHangfireScheduler(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/jobs", new DashboardOptions()
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
});

app.MapControllers();

app.Run();
