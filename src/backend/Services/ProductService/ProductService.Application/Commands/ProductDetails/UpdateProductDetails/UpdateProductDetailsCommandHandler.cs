using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.ProductDetails.UpdateProductDetails
{
    public class UpdateProductDetailsCommandHandler : IRequestHandler<UpdateProductDetailsCommand, ProductDetailsResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductDetailsRepository _productDetailsRepository;
        private readonly ILogger<UpdateProductDetailsCommandHandler> _logger;

        public UpdateProductDetailsCommandHandler(
            IMapper mapper,
            IProductDetailsRepository productDetailsRepository,
            ILogger<UpdateProductDetailsCommandHandler> logger)
        {
            _mapper = mapper;
            _productDetailsRepository = productDetailsRepository;
            _logger = logger;
        }

        public async Task<ProductDetailsResponseDTO> Handle(UpdateProductDetailsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating details for product @{id}", request.RequestDTO.ProductId);

            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.RequestDTO.ProductId, cancellationToken);

            if (existingDetails is null)
            {
                throw new NotFoundException("Details for given product not found.");
            }

            _mapper.Map(request.RequestDTO, existingDetails);

            existingDetails = await _productDetailsRepository.UpdateAsync(existingDetails, cancellationToken);

            _logger.LogInformation("Successfully updated details for product @{id}", request.RequestDTO.ProductId);

            return _mapper.Map<ProductDetailsResponseDTO>(existingDetails);
        }
    }
}
