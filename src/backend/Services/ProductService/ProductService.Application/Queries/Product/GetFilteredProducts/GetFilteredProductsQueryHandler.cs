using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Queries.Product.GetFilteredProducts
{
    public class GetFilteredProductsQueryHandler : IRequestHandler<GetFilteredProductsQuery, PaginatedResponseDTO<ProductResponseDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetFilteredProductsQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<PaginatedResponseDTO<ProductResponseDTO>> Handle(GetFilteredProductsQuery request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            var normalizedTerm = dto.SearchTerm?.Trim().ToLower();

            Expression<Func<Domain.Entities.Product, bool>> filter = product =>
                (string.IsNullOrWhiteSpace(normalizedTerm) ||
                    product.Title.ToLower().Contains(normalizedTerm) ||
                    product.Manufacturer.NormalizedName.Contains(normalizedTerm)) &&

                (dto.CategoryIds == null || dto.CategoryIds.Count == 0 ||
                    product.Categories.Any(c => dto.CategoryIds.Contains(c.Id))
              );

            var data = await _productRepository.ListWithNestedAsync(
                dto.Page.PageNumber,
                dto.Page.PageSize,
                filter,
                cancellationToken);

            if (data.Items.Count == 0)
            {
                throw new NotFoundException("No products matching the given filter found.");
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
