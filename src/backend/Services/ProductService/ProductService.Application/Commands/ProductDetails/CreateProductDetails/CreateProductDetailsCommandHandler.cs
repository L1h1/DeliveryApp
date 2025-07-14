using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CreateProductDetailsCommandHandler> _logger;

        public CreateProductDetailsCommandHandler(
            IMapper mapper,
            IProductRepository productRepository,
            IProductDetailsRepository productDetailsRepository,
            ILogger<CreateProductDetailsCommandHandler> logger)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _productDetailsRepository = productDetailsRepository;
            _logger = logger;
        }

        public async Task<ProductDetailsResponseDTO> Handle(CreateProductDetailsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking for product @{id} existence", request.RequestDTO.ProductId);

            var existingProduct = await _productRepository.GetByIdAsync(request.RequestDTO.ProductId, cancellationToken);

            if (existingProduct is null)
            {
                throw new NotFoundException("Product with given id not found.");
            }

            _logger.LogInformation("Checking for details for given product @{id} existence", request.RequestDTO.ProductId);

            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.RequestDTO.ProductId, cancellationToken);

            if (existingDetails is not null)
            {
                throw new BadRequestException("Details for this product already exist. Use update flow instead.");
            }

            var details = _mapper.Map<Domain.Entities.ProductDetails>(request.RequestDTO);

            details = await _productDetailsRepository.AddAsync(details, cancellationToken);

            _logger.LogInformation("Successfully created details for product @{id}", request.RequestDTO.ProductId);

            return _mapper.Map<ProductDetailsResponseDTO>(details);
        }
    }
}
