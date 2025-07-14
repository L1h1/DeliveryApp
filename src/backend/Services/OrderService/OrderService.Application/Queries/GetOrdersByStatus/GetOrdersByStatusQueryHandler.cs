using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Queries.GetOrdersByStatus
{
    public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, List<OrderResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrdersByStatusQueryHandler> _logger;

        public GetOrdersByStatusQueryHandler(IMapper mapper, IOrderRepository orderRepository, ILogger<GetOrdersByStatusQueryHandler> logger)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<List<OrderResponseDTO>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving orders with status @{status}", request.OrderStatus);

            var response = await _orderRepository.ListAsync(o => o.OrderStatus == request.OrderStatus, cancellationToken);

            if (response.Count == 0)
            {
                throw new NotFoundException("No orders with given status found.");
            }

            _logger.LogInformation("Successfully retrieved orders with status @{status}", request.OrderStatus);

            return _mapper.Map<List<OrderResponseDTO>>(response);
        }
    }
}
