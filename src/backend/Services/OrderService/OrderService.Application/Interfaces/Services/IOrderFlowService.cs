using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces.Services
{
    public interface IOrderFlowService
    {
        Task ProcessOrderCreation(Order order, string userEmail, CancellationToken cancellationToken = default);
    }
}
