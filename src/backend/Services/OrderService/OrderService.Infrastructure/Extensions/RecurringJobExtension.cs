using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;

namespace OrderService.Infrastructure.Extensions
{
    public static class RecurringJobExtension
    {
        public static void RunRecurringJobs(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            var backgroundJobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
            var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            backgroundJobService.CreateRepeatedJob("rec_monthlyOrderClear", () => repository.DeleteOldOrdersAsync(CancellationToken.None), Cron.Monthly());
        }
    }
}
