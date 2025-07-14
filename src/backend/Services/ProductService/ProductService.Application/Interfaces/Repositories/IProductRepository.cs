using System.Linq.Expressions;
using ProductService.Application.DTOs.Response;
using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<PaginatedResponseDTO<Product>> ListWithNestedAsync(int pageNumber, int pageSize, Expression<Func<Product, bool>>? filter = null, CancellationToken cancellationToken = default);
    }
}
