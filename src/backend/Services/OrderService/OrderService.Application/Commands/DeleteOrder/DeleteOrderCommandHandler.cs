using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;
        private readonly IDistributedCache _distributedCache;
        
        public DeleteOrderCommandHandler(IOrderRepository orderRepository, ILogger<DeleteOrderCommandHandler> logger, IDistributedCache distributedCache)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _distributedCache = distributedCache;
        }
       
        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting order @{id}", request.OrderId);

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            await _orderRepository.DeleteAsync(request.OrderId, cancellationToken);
            await _distributedCache.RemoveAsync($"order:{request.OrderId}");
            await _distributedCache.RemoveAsync($"orders:client:{order.ClientId}");

            _logger.LogInformation("Successfully deleted order @{id}", request.OrderId);

            return Unit.Value;
        }
    }
}
