using System.Linq.Expressions;
using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Order>> ListAsync(Expression<Func<Order, bool>> filter, CancellationToken cancellationToken = default);
        Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteOldOrdersAsync(CancellationToken cancellationToken = default);
    }
}
