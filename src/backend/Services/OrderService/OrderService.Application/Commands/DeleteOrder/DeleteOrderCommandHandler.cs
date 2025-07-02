using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDistributedCache _distributedCache;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository, IDistributedCache distributedCache)
        {
            _orderRepository = orderRepository;
            _distributedCache = distributedCache;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            await _orderRepository.DeleteAsync(request.OrderId, cancellationToken);
            await _distributedCache.RemoveAsync($"order:{request.OrderId}");
            await _distributedCache.RemoveAsync($"orders:client:{order.ClientId}");

            return Unit.Value;
        }
    }
}
