using AutoMapper;
using MediatR;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;

namespace OrderService.Application.Queries.GetOrdersByClientId
{
    public class GetOrdersByClientIdQueryHandler : IRequestHandler<GetOrdersByClientIdQuery, List<OrderResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IOrderRepository _orderRepository;

        public GetOrdersByClientIdQueryHandler(IMapper mapper, IUserService userService, IOrderRepository orderRepository)
        {
            _mapper = mapper;
            _userService = userService;
            _orderRepository = orderRepository;
        }

        public async Task<List<OrderResponseDTO>> Handle(GetOrdersByClientIdQuery request, CancellationToken cancellationToken)
        {
            var existingUser = await _userService.GetByIdAsync(request.Id.ToString(), cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException("User with given id not found.");
            }

            var response = await _orderRepository.ListAsync(o => o.ClientId == request.Id, cancellationToken);

            if (response.Count == 0)
            {
                throw new NotFoundException("No orders found.");
            }

            return _mapper.Map<List<OrderResponseDTO>>(response);
        }
    }
}
