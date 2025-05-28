using AutoMapper;
using MediatR;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Application.Queries.GetOrdersByClientId
{
    public class GetOrdersByClientIdQueryHandler : IRequestHandler<GetOrdersByClientIdQuery, List<OrderResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByClientIdQueryHandler(IMapper mapper, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderResponseDTO>> Handle(GetOrdersByClientIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: check user existence when gRPC communication is implemented
            var response = await _orderRepository.ListAsync(o => o.Id == request.Id, cancellationToken);

            if (response.Count == 0)
            {
                throw new NotFoundException("No orders found.");
            }

            return _mapper.Map<List<OrderResponseDTO>>(response);
        }
    }
}
