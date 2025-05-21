using System.Linq.Expressions;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> AddAsync(T tEntity, CancellationToken cancellationToken = default);
        Task<T?> UpdateAsync(T tEntity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T tEntity, CancellationToken cancellationToken = default);
        Task<ICollection<T>> ListAsync(Expression<Func<T, bool>>? filter, CancellationToken cancellationToken = default);
    }
}
