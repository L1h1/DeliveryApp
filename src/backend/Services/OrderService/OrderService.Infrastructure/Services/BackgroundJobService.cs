using System.Linq.Expressions;
using Hangfire;
using OrderService.Application.Interfaces.Services;

namespace OrderService.Infrastructure.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public BackgroundJobService(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        public void CreateRepeatedJob(string jobId, Expression<Action> method, string cronExpression)
        {
            _recurringJobManager.AddOrUpdate(jobId, method, cronExpression);
        }

        public void RemoveRepeatedJob(string jobId)
        {
            _recurringJobManager.RemoveIfExists(jobId);
        }
    }
}
