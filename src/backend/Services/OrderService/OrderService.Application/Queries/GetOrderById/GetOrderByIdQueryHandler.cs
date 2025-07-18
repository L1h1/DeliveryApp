using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _distributedCache;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<GetOrderByIdQueryHandler> logger, IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<DetailedOrderResponseDTO> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving order @{id}", request.OrderId);

            var cacheKey = $"order:{request.OrderId}";
            var cached = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (cached is not null)
            {
                return JsonSerializer.Deserialize<DetailedOrderResponseDTO>(cached);
            }

            var response = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (response is null)
            {
                throw new NotFoundException("Order with given id not found.");
            }

            _logger.LogInformation("Successfully retrieved order @{id}", request.OrderId);

            var result = _mapper.Map<DetailedOrderResponseDTO>(response);

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
