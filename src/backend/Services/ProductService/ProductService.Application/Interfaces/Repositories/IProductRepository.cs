using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken = default);
        Task<ICollection<Product>> ListWithQueryAsync(Func<IQueryable<Product>, IQueryable<Product>>? query, CancellationToken cancellationToken = default);
    }
}
