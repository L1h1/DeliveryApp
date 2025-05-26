using MediatR;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;

        public DeleteProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct is null)
            {
                throw new NotFoundException("Prodct with given id not found.");
            }

            await _productRepository.DeleteAsync(existingProduct, cancellationToken);

            return Unit.Value;
        }
    }
}
