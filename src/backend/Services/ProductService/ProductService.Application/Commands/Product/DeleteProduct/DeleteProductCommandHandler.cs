using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IProductRepository productRepository, ILogger<DeleteProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting product @{id}", request.Id);

            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct is null)
            {
                throw new NotFoundException("Prodct with given id not found.");
            }

            await _productRepository.DeleteAsync(existingProduct, cancellationToken);

            _logger.LogInformation("Successfully deleted product @{id}", request.Id);

            return Unit.Value;
        }
    }
}
