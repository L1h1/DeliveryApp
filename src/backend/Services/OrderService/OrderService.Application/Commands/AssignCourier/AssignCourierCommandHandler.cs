using MediatR;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Commands.AssignCourier
{
    public class AssignCourierCommandHandler : IRequestHandler<AssignCourierCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public AssignCourierCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            // TODO: check user existence when gRPC communication is implemented
            order.CourierId = request.CourierId;
            order.OrderStatus = Domain.Enums.OrderStatus.Assigned;

            await _orderRepository.UpdateAsync(order, cancellationToken);

            return Unit.Value;
        }
    }
}
