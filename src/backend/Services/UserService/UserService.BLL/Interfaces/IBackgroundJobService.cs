using System.Linq.Expressions;

namespace UserService.BLL.Interfaces
{
    public interface IBackgroundJobService
    {
        void CreateJob(Expression<Action> method);
    }
}
