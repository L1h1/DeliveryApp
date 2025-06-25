using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;

namespace OrderService.Application.Commands.AssignCourier
{
    public class AssignCourierCommandHandler : IRequestHandler<AssignCourierCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserService _userService;
        private readonly ILogger<AssignCourierCommandHandler> _logger;

        public AssignCourierCommandHandler(IOrderRepository orderRepository, IUserService userService, ILogger<AssignCourierCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<Unit> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Assigning courier @{courierId} to order @{orderId}", request.CourierId, request.OrderId);

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

            _logger.LogInformation("Successfully assigned courier @{courierId} to order @{orderId}", request.CourierId, request.OrderId);

            return Unit.Value;
        }
    }
}
