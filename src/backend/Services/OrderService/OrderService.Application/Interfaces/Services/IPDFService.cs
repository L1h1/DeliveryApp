using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces.Services
{
    public interface IPDFService
    {
        Task<string> CreateDocumentAsync(Order order, CancellationToken cancellationToken = default);
    }
}
