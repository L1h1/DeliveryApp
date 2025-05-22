using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Product.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedResponseDTO<ProductResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetAllProductsQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<PaginatedResponseDTO<ProductResponseDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var data = await _productRepository.ListWithNestedAsync(
                request.dto.PageNumber,
                request.dto.PageSize,
                cancellationToken: cancellationToken);

            if (!data.Items.Any())
            {
                throw new NotFoundException("No products found.");
            }

            return new PaginatedResponseDTO<ProductResponseDTO>
            {
                Items = _mapper.Map<ICollection<ProductResponseDTO>>(data.Items),
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
            };
        }
    }
}
