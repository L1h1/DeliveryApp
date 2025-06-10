using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces.Services
{
    public interface IBillService
    {
        Task<string> CreateDocumentAsync(Order order, CancellationToken cancellationToken = default);
    }
}
