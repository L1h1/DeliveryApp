using System.Text.Json;
using AutoMapper;
using MediatR;
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
        private readonly IDistributedCache _distributedCache;

        public GetProductByIdQueryHandler(IMapper mapper, IProductRepository productRepository, IProductDetailsRepository productDetailsRepository, IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productDetailsRepository = productDetailsRepository;
            _distributedCache = distributedCache;
        }

        public async Task<DetailedProductResponseDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
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

            var productDetails = await _productDetailsRepository.GetByIdAsync(request.Id, cancellationToken);

            if (productDetails is null)
            {
                throw new NotFoundException("No details found for the given product.");
            }

            var result = _mapper.Map(productDetails, _mapper.Map<DetailedProductResponseDTO>(generalData));
            var serialized = JsonSerializer.Serialize(result);

            await _distributedCache.SetStringAsync(cacheKey, serialized, cancellationToken);

            return result;
        }
    }
}
