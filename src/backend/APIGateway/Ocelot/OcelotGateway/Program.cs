using JwtAuthentication;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway.Authorizing;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AdminOrCourierRoleCheckHandler>();

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
