using System.Linq.Expressions;

namespace OrderService.Application.Interfaces.Services
{
    public interface IBackgroundJobService
    {
        void CreateRepeatedJob(string jobId, Expression<Action> method, string cronExpression);
        void RemoveRepeatedJob(string jobId);
    }
}
