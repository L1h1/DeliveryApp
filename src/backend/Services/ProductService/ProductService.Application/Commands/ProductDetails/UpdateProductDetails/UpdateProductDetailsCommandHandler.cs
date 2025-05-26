using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.ProductDetails.UpdateProductDetails
{
    public class UpdateProductDetailsCommandHandler : IRequestHandler<UpdateProductDetailsCommand, ProductDetailsResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductDetailsRepository _productDetailsRepository;

        public UpdateProductDetailsCommandHandler(IMapper mapper, IProductDetailsRepository productDetailsRepository)
        {
            _mapper = mapper;
            _productDetailsRepository = productDetailsRepository;
        }

        public async Task<ProductDetailsResponseDTO> Handle(UpdateProductDetailsCommand request, CancellationToken cancellationToken)
        {
            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.requestDTO.ProductId, cancellationToken);

            if (existingDetails is null)
            {
                throw new NotFoundException("Details for given product not found.");
            }

            _mapper.Map(request.requestDTO, existingDetails);

            existingDetails = await _productDetailsRepository.UpdateAsync(existingDetails, cancellationToken);

            return _mapper.Map<ProductDetailsResponseDTO>(existingDetails);
        }
    }
}
