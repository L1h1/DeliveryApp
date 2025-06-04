using AutoMapper;
using MediatR;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entities;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IBillService _billService;

        public CreateOrderCommandHandler(
            IMapper mapper, 
            IOrderRepository orderRepository,
            IProductService productService,
            IUserService userService,
            IBackgroundJobService backgroundJobService,
            IBillService billService)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _productService = productService;
            _userService = userService;
            _backgroundJobService = backgroundJobService;
            _billService = billService;
        }

        public async Task<OrderResponseDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userService.GetByIdAsync(request.RequestDTO.ClientId.ToString(), cancellationToken);

            if (existingUser is null)
            {
                throw new NotFoundException("Client with given id not found.");
            }

            var productIds = request.RequestDTO.Items.Select(x => x.ProductId).ToList();
            var products = await _productService.GetProductsAsync(productIds, cancellationToken);

            if (products.Count != productIds.Count)
            {
                throw new NotFoundException("Some products where not found.");
            }

            var order = _mapper.Map<Order>(request.RequestDTO);

            order.Items = order.Items.Join(
                products,
                dtoCollection => dtoCollection.ProductId,
                serverCollection => serverCollection.Id,
                (dtoCollection, serverCollection) => new OrderItem
                {
                    ProductId = dtoCollection.ProductId,
                    Quantity = dtoCollection.Quantity,
                    Price = serverCollection.Price,
                    Title = serverCollection.Title,
                })
                .ToList();

            order.TotalPrice = order.Items.Sum(x => x.Quantity * x.Price);

            await _orderRepository.CreateAsync(order, cancellationToken);
            _backgroundJobService.CreateJob(() => _billService.CreateBill(order, cancellationToken));

            return _mapper.Map<OrderResponseDTO>(order);
        }
    }
}
