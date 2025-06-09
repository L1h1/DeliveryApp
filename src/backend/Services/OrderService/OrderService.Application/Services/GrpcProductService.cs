using AutoMapper;
using OrderService.Application.DTOs.Response;
using OrderService.Application.Interfaces.Services;
using OrderService.Application.Protos;

namespace OrderService.Application.Services
{
    public class GrpcProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly ProductService.ProductServiceClient _productServiceClient;

        public GrpcProductService(IMapper mapper, ProductService.ProductServiceClient productServiceClient)
        {
            _mapper = mapper;
            _productServiceClient = productServiceClient;
        }

        public async Task<List<ProductResponseDTO>> GetProductsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
        {
            var request = new ProductsByIdsRequest();
            request.Ids.AddRange(ids.Select(x => x.ToString()));

            var response = await _productServiceClient.GetByIdsAsync(request, cancellationToken: cancellationToken);

            return _mapper.Map<List<ProductResponseDTO>>(response.Products);
        }
    }
}
