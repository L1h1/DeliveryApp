using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Infrastructure.Data.SQL;

namespace ProductService.Infrastructure.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using EFDbContext context = scope.ServiceProvider.GetRequiredService<EFDbContext>();

            context.Database.Migrate();
        }
    }
}
