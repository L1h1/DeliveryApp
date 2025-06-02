using OrderService.Application.DTOs.Response;

namespace OrderService.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductResponseDTO>> GetProductsAsync(List<Guid> ids, CancellationToken cancellationToken = default);
    }
}
