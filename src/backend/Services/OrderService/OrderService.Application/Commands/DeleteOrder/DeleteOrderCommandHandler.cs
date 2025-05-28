using MediatR;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            await _orderRepository.DeleteAsync(request.OrderId, cancellationToken);

            return Unit.Value;
        }
    }
}
