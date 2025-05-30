using System.Linq.Expressions;
using ProductService.Application.DTOs.Response;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> AddAsync(T tEntity, CancellationToken cancellationToken = default);
        Task<T?> UpdateAsync(T tEntity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T tEntity, CancellationToken cancellationToken = default);
        Task<PaginatedResponseDTO<T>> ListAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default);
    }
}
