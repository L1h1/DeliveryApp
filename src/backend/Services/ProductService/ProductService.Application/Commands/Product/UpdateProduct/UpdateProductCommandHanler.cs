using AutoMapper;
using MediatR;
using ProductService.Application.DTOs.Response;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandHanler : IRequestHandler<UpdateProductCommand, ProductResponseDTO>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHanler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<ProductResponseDTO> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.id, cancellationToken);

            if (existingProduct is null)
            {
                throw new NotFoundException("Product with given id not found.");
            }

            _mapper.Map(request.requestDTO, existingProduct);
            existingProduct = await _productRepository.UpdateAsync(existingProduct, cancellationToken);

            return _mapper.Map<ProductResponseDTO>(existingProduct);
        }
    }
}
