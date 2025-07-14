using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<GetOrdersByClientIdQueryHandler> _logger;

        public GetOrdersByClientIdQueryHandler(
            IMapper mapper,
            IUserService userService,
            IOrderRepository orderRepository,
            ILogger<GetOrdersByClientIdQueryHandler> logger,
            IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _userService = userService;
            _orderRepository = orderRepository;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<List<OrderResponseDTO>> Handle(GetOrdersByClientIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving orders for client @{id}", request.Id);

            var cacheKey = $"orders:client:{request.Id}";
            var cached = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cached))
            {
                return JsonSerializer.Deserialize<List<OrderResponseDTO>>(cached);
            }

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
           _logger.LogInformation("Successfully retrieved orders for client @{id}", request.Id);
            
            var result = _mapper.Map<List<OrderResponseDTO>>(response);

            await _distributedCache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                }, cancellationToken);
            
            return result;
        }
    }
}
