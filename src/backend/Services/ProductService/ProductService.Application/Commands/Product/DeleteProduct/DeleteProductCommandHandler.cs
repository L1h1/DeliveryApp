using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<DeleteProductCommandHandler> _logger;
        private readonly IDistributedCache _distributedCache;
        
        public DeleteProductCommandHandler(IProductRepository productRepository, ILogger<DeleteProductCommandHandler> logger, IDistributedCache distributedCache)
        {
            _productRepository = productRepository;
            _logger = logger;
            _distributedCache = distributedCache;
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
            await _distributedCache.RemoveAsync($"product:{request.Id}", cancellationToken);

            _logger.LogInformation("Successfully deleted product @{id}", request.Id);

            return Unit.Value;
        }
    }
}
