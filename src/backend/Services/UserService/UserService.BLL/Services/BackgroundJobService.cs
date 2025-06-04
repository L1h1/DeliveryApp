using System.Linq.Expressions;
using Hangfire;
using UserService.BLL.Interfaces;

namespace UserService.BLL.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public BackgroundJobService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public void CreateJob(Expression<Action> method)
        {
            _backgroundJobClient.Enqueue(method);
        }

    }
}
