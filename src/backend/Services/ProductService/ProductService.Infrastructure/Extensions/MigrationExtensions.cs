using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Infrastructure.Data.SQL;

namespace ProductService.Infrastructure.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrations(this IApplicationBuilder app, CancellationToken cancellationToken = default)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using EFDbContext context = scope.ServiceProvider.GetRequiredService<EFDbContext>();

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync(cancellationToken);
            }
        }
    }
}
