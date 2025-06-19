using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.ProductDetails.UpdateProductDetails
{
    public class UpdateProductDetailsCommandHandler : IRequestHandler<UpdateProductDetailsCommand, ProductDetailsResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductDetailsRepository _productDetailsRepository;
        private readonly IDistributedCache _distributedCache;

        public UpdateProductDetailsCommandHandler(IMapper mapper, IProductDetailsRepository productDetailsRepository, IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _productDetailsRepository = productDetailsRepository;
            _distributedCache = distributedCache;
        }

        public async Task<ProductDetailsResponseDTO> Handle(UpdateProductDetailsCommand request, CancellationToken cancellationToken)
        {
            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.RequestDTO.ProductId, cancellationToken);

            if (existingDetails is null)
            {
                throw new NotFoundException("Details for given product not found.");
            }

            _mapper.Map(request.RequestDTO, existingDetails);

            existingDetails = await _productDetailsRepository.UpdateAsync(existingDetails, cancellationToken);
            await _distributedCache.RemoveAsync($"product:{request.RequestDTO.ProductId}", cancellationToken);

            return _mapper.Map<ProductDetailsResponseDTO>(existingDetails);
        }
    }
}
