using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;
        private readonly IDistributedCache _distributedCache;
        
        public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, ILogger<UpdateOrderStatusCommandHandler> logger, IDistributedCache distributedCache)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating order @{orderId} with status @{status}", request.OrderId, request.OrderStatus);

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            order.OrderStatus = request.OrderStatus;

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _distributedCache.RemoveAsync($"order:{request.OrderId}");
            await _distributedCache.RemoveAsync($"orders:client:{order.ClientId}");

            _logger.LogInformation("Successfully updated order @{orderId} with status @{status}", request.OrderId, request.OrderStatus);

            return Unit.Value;
        }
    }
}
