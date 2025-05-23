using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<Category?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
        Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}
