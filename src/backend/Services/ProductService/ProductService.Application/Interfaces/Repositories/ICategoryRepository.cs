using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<Category?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ICollection<Category>>? ListByIdsAsync(ICollection<Guid> ids, CancellationToken cancellationToken = default);
    }
}
