using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Product.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedResponseDTO<ProductResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(IMapper mapper, IProductRepository productRepository, ILogger<GetAllProductsQueryHandler> logger)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponseDTO<ProductResponseDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving product list page @{page}", request.Dto);

            var data = await _productRepository.ListWithNestedAsync(
                request.Dto.PageNumber,
                request.Dto.PageSize,
                cancellationToken: cancellationToken);

            if (data.Items.Count == 0)
            {
                throw new NotFoundException("No products found.");
            }

            _logger.LogInformation("Successfully retrieved product list page @{page}", request.Dto);

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
