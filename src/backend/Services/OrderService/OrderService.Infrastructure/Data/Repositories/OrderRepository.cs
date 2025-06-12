using System.Linq.Expressions;
using MongoDB.Driver;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MongoDbContext _context;

        public OrderRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.InsertOneAsync(order, cancellationToken: cancellationToken);

            return order;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _context.Orders.DeleteOneAsync(o => o.Id == id, cancellationToken: cancellationToken);
        }

        public async Task DeleteOldOrdersAsync(CancellationToken cancellationToken = default)
        {
            await _context.Orders.DeleteManyAsync(
                o => o.OrderStatus == Domain.Enums.OrderStatus.Delivered &&
                o.CreatedAt <= DateTime.UtcNow.AddMonths(-1), cancellationToken: cancellationToken);
        }

        public async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _context.Orders.Find(o => o.Id == id).FirstOrDefaultAsync(cancellationToken);

            return response;
        }

        public async Task<List<Order>> ListAsync(Expression<Func<Order, bool>> filter, CancellationToken cancellationToken = default)
        {
            var response = await _context.Orders.Find(filter).ToListAsync(cancellationToken);

            return response;
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.ReplaceOneAsync(o => o.Id == order.Id, order, cancellationToken: cancellationToken);
        }
    }
}
