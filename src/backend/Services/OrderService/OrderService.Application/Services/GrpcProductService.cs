using AutoMapper;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Protos;

namespace OrderService.Application.Services
{
    public class GrpcProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly ProductService.ProductServiceClient _productServiceClient;
        private readonly ILogger<GrpcProductService> _logger;

        public GrpcProductService(IMapper mapper, ProductService.ProductServiceClient productServiceClient, ILogger<GrpcProductService> logger)
        {
            _mapper = mapper;
            _productServiceClient = productServiceClient;
            _logger = logger;
        }

        public async Task<List<ProductResponseDTO>> GetProductsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Requesting data via gRPC for @{count} products", ids.Count);

            var request = new ProductsByIdsRequest();
            request.Ids.AddRange(ids.Select(x => x.ToString()));

            var response = await _productServiceClient.GetByIdsAsync(request, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully retrieved data via gRPC for @{count} products", ids.Count);

            return _mapper.Map<List<ProductResponseDTO>>(response.Products);
        }
    }
}
