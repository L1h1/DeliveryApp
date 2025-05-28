using AutoMapper;
using MediatR;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Enums;

namespace OrderService.Application.Queries.GetOrdersByCourierId
{
    public class GetOrdersByCourierIdQueryHandler : IRequestHandler<GetOrdersByCourierIdQuery, List<OrderResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByCourierIdQueryHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderResponseDTO>> Handle(GetOrdersByCourierIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: check user existence when gRPC communication is implemented
            var response = await _orderRepository.ListAsync(
                    o => o.Id == request.Id &&
                    (o.OrderStatus == OrderStatus.Assigned || o.OrderStatus == OrderStatus.Delivering),
                    cancellationToken);

            if (response.Count == 0)
            {
                throw new NotFoundException("No orders found.");
            }

            return _mapper.Map<List<OrderResponseDTO>>(response);
        }
    }
}
