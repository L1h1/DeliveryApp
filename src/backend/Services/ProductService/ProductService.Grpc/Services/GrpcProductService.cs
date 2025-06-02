using Grpc.Core;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Grpc.Services
{
    public class GrpcProductService : ProductService.ProductServiceBase
    {
        private readonly IProductRepository _productRepository;

        public GrpcProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public override async Task<ProductsResponse> GetByIds(ProductsByIdsRequest request, ServerCallContext context)
        {
            var guids = request.Ids.Select(Guid.Parse).ToList();
            var data = await _productRepository.ListAsync(1, 100, p => guids.Contains(p.Id));

            if (data.TotalCount != guids.Count)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Some products where not found."));
            }

            var response = new ProductsResponse();
            response.Products.AddRange(data.Items.Select(p => new ProductResponse
            {
                Id = p.Id.ToString(),
                Title = p.Title,
                Price = (double)p.Price,
            }));

            return response;
        }
    }
}
