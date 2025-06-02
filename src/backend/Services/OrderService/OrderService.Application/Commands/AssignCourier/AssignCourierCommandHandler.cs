using MediatR;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;

namespace OrderService.Application.Commands.AssignCourier
{
    public class AssignCourierCommandHandler : IRequestHandler<AssignCourierCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserService _userService;

        public AssignCourierCommandHandler(IOrderRepository orderRepository, IUserService userService)
        {
            _orderRepository = orderRepository;
            _userService = userService;
        }

        public async Task<Unit> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userService.GetByIdAsync(request.CourierId.ToString(), cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException("Courier with given id not found.");
            }

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            order.CourierId = request.CourierId;
            order.OrderStatus = Domain.Enums.OrderStatus.Assigned;

            await _orderRepository.UpdateAsync(order, cancellationToken);

            return Unit.Value;
        }
    }
}
