using System.Linq.Expressions;
using MongoDB.Driver;
using OrderService.Application.Interfaces.Caching;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MongoDbContext _context;
        private readonly ICacheService _cacheService;

        public OrderRepository(MongoDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.InsertOneAsync(order, cancellationToken: cancellationToken);

            await _cacheService.RemoveAsync($"Orders:{order.ClientId}", cancellationToken);

            return order;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders.FindOneAndDeleteAsync(o => o.Id == id, cancellationToken: cancellationToken);

            await _cacheService.RemoveAsync($"Orders:{order.ClientId}", cancellationToken);
        }

        public async Task DeleteOldOrdersAsync(CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow.AddMonths(-1);
            var filter = Builders<Order>.Filter.Where(
                o => o.OrderStatus == Domain.Enums.OrderStatus.Delivered &&
                     o.CreatedAt <= cutoffDate);

            var oldOrders = await _context.Orders.Find(filter)
                .Project(o => new { o.Id, o.ClientId })
                .ToListAsync(cancellationToken);

            foreach (var order in oldOrders)
            {
                await _cacheService.RemoveAsync($"Orders:{order.ClientId}", cancellationToken);
            }

            await _context.Orders.DeleteManyAsync(filter, cancellationToken: cancellationToken);
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

        public async Task<List<Order>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var key = $"Orders:{userId}";
            var cached = await _cacheService.GetAsync<List<Order>>(key, cancellationToken);

            if (cached is not null)
            {
                return cached;
            }

            var response = await _context.Orders.Find(o => o.ClientId == userId).ToListAsync(cancellationToken);

            await _cacheService.SetAsync(key, response, cancellationToken);

            return response;
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.ReplaceOneAsync(o => o.Id == order.Id, order, cancellationToken: cancellationToken);

            await _cacheService.RemoveAsync($"Orders:{order.ClientId}", cancellationToken);
        }
    }
}
