using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<Category?> GetByNameAsync(string categoryName, CancellationToken cancellationToken = default);
    }
}
