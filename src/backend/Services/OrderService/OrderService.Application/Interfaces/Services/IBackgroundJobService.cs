using System.Linq.Expressions;

namespace OrderService.Application.Interfaces.Services
{
    public interface IBackgroundJobService
    {
        string CreateJob(Expression<Action> method);
        void CreateRepeatedJob(string jobId, Expression<Action> method, string cronExpression);
        void RemoveRepeatedJob(string jobId);
        void CreateContinuationJob<T>(string jobId, Expression<Action<T>> method);
    }
}
