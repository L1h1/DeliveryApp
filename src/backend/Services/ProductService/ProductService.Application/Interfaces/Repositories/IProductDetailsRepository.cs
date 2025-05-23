using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IProductDetailsRepository : IBaseRepository<ProductDetails>
    {
        Task<ProductDetails> GetByIdAsync(string id, CancellationToken cancellationToken);
    }
}
