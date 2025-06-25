using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, DetailedOrderResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<GetOrderByIdQueryHandler> logger)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<DetailedOrderResponseDTO> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving order @{id}", request.OrderId);

            var response = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (response is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            _logger.LogInformation("Successfully retrieved order @{id}", request.OrderId);

            return _mapper.Map<DetailedOrderResponseDTO>(response);
        }
    }
}
