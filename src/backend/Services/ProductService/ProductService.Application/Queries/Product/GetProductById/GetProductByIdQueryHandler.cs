using AutoMapper;
using MediatR;
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

        public GetProductByIdQueryHandler(IMapper mapper, IProductRepository productRepository, IProductDetailsRepository productDetailsRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productDetailsRepository = productDetailsRepository;
        }

        public async Task<DetailedProductResponseDTO> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var generalData = await _productRepository.GetByIdAsync(request.id);

            if (generalData is null)
            {
                throw new NotFoundException("Product not found.");
            }

            var productDetails = await _productDetailsRepository.GetByIdAsync(generalData.Id, cancellationToken);

            if (productDetails is null)
            {
                throw new NotFoundException("No details found for the given product.");
            }

            return _mapper.Map(productDetails, _mapper.Map<DetailedProductResponseDTO>(generalData));
        }
    }
}
