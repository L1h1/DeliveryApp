using OrderService.Domain.Entities;
using System.Linq.Expressions;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Order>> ListAsync(Expression<Func<Order, bool>> filter, CancellationToken cancellationToken = default);
        Task CreateAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
