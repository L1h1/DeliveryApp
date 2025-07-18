using JwtAuthentication;
using Microsoft.Extensions.FileProviders;
using ProductService.API;
using ProductService.API.Middleware;
using ProductService.Application;
using ProductService.Infrastructure;
using ProductService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.AddSerilogLogging();

// Add services to the container.
builder.Services
    .AddApplication()
    .AddOptions(builder.Configuration)
    .AddDataAccess(builder.Configuration)
    .AddRedisCaching(builder.Configuration)
    .AddOptions(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrations();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads",
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
