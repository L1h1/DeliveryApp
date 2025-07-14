using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Enums;

namespace OrderService.Application.Queries.GetOrdersByCourierId
{
    public class GetOrdersByCourierIdQueryHandler : IRequestHandler<GetOrdersByCourierIdQuery, List<OrderResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrdersByCourierIdQueryHandler> _logger;

        public GetOrdersByCourierIdQueryHandler(
            IMapper mapper,
            IUserService userService,
            IOrderRepository orderRepository,
            ILogger<GetOrdersByCourierIdQueryHandler> logger)
        {
            _mapper = mapper;
            _userService = userService;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<List<OrderResponseDTO>> Handle(GetOrdersByCourierIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving orders for courier @{id}", request.Id);

            var existingUser = await _userService.GetByIdAsync(request.Id.ToString(), cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException("User with given id not found.");
            }

            var response = await _orderRepository.ListAsync(
                    o => o.CourierId == request.Id &&
                    (o.OrderStatus == OrderStatus.Assigned || o.OrderStatus == OrderStatus.Delivering),
                    cancellationToken);

            if (response.Count == 0)
            {
                throw new NotFoundException("No orders found.");
            }

            _logger.LogInformation("Successfully retrieved orders for courier @{id}", request.Id);

            return _mapper.Map<List<OrderResponseDTO>>(response);
        }
    }
}
