using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Product.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, DetailedProductResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailsRepository _productDetailsRepository;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;
        private readonly IDistributedCache _distributedCache;
        
        public GetProductByIdQueryHandler(
            IMapper mapper,
            IProductRepository productRepository,
            IProductDetailsRepository productDetailsRepository,
            ILogger<GetProductByIdQueryHandler> logger,
            IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productDetailsRepository = productDetailsRepository;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        public async Task<DetailedProductResponseDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving general product @{id} data", request.Id);

            var cacheKey = $"product:{request.Id}";
            var cachedData = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<DetailedProductResponseDTO>(cachedData)!;
            }

            var generalData = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (generalData is null)
            {
                throw new NotFoundException("Product not found.");
            }

            _logger.LogInformation("Retrieving details for product @{id}", request.Id);

            var productDetails = await _productDetailsRepository.GetByIdAsync(request.Id, cancellationToken);

            if (productDetails is null)
            {
                throw new NotFoundException("No details found for the given product.");
            }

            _logger.LogInformation("Successfully retrived all product @{id} data", request.Id);

            var result = _mapper.Map(productDetails, _mapper.Map<DetailedProductResponseDTO>(generalData));

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
