using AutoMapper;
using MediatR;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Queries.GetOrdersByStatus
{
    public class GetOrdersByStatusQueryHandler : IRequestHandler<GetOrdersByStatusQuery, List<OrderResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByStatusQueryHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderResponseDTO>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
        {
            var response = await _orderRepository.ListAsync(o => o.OrderStatus == request.OrderStatus, cancellationToken);

            if (response.Count == 0)
            {
                throw new NotFoundException("No orders with given status found.");
            }

            return _mapper.Map<List<OrderResponseDTO>>(response);
        }
    }
}
