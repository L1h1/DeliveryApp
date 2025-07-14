using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Application.DTOs.Messaging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces.Messaging.Producers;
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
        private readonly IMessageProducer _messageProducer;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IDistributedCache _distributedCache;

        public CreateOrderCommandHandler(
            IMapper mapper,
            IOrderRepository orderRepository,
            IProductService productService,
            IUserService userService,
            IBackgroundJobService backgroundJobService,
            IBillService billService,
            IMessageProducer messageProducer,
            ILogger<CreateOrderCommandHandler> logger)
            IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _productService = productService;
            _userService = userService;
            _backgroundJobService = backgroundJobService;
            _billService = billService;
            _messageProducer = messageProducer;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<OrderResponseDTO> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating order for client @{id}", request.RequestDTO.ClientId);

            var userEmail = await _userService.GetByIdAsync(request.RequestDTO.ClientId.ToString(), cancellationToken);

            if (userEmail is null)
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

            _logger.LogInformation("Scheduling bill email job for order @{id}", order.Id);

            _backgroundJobService.CreateJob(() => GenerateAndSendBillAsync(order, userEmail));

            _logger.LogInformation("Successfully created order @{orderId} for client @{clientId}", order.Id, order.ClientId);
            
            await _distributedCache.RemoveAsync($"orders:client:{order.ClientId}");

            return _mapper.Map<OrderResponseDTO>(order);
        }

        public async Task GenerateAndSendBillAsync(Order order, string email)
        {
            var contents = await _billService.CreateDocumentAsync(order);

            await _messageProducer.SendMessageAsync("bills", new BillDTO
            {
                Email = email,
                Contents = contents
            });
        }
    }
}
