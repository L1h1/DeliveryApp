using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.ProductDetails.CreateProductDetails
{
    public class CreateProductDetailsCommandHandler : IRequestHandler<CreateProductDetailsCommand, ProductDetailsResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailsRepository _productDetailsRepository;

        public CreateProductDetailsCommandHandler(IMapper mapper, IProductRepository productRepository, IProductDetailsRepository productDetailsRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productDetailsRepository = productDetailsRepository;
        }

        public async Task<ProductDetailsResponseDTO> Handle(CreateProductDetailsCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.requestDTO.ProductId, cancellationToken);

            if (existingProduct is null)
            {
                throw new NotFoundException("Product with given id not found.");
            }

            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.requestDTO.ProductId, cancellationToken);

            if (existingDetails is not null)
            {
                throw new BadRequestException("Details for this product already exist. Use update flow instead.");
            }

            var details = _mapper.Map<Domain.Entities.ProductDetails>(request.requestDTO);

            details = await _productDetailsRepository.AddAsync(details, cancellationToken);

            return _mapper.Map<ProductDetailsResponseDTO>(details);
        }
    }
}
