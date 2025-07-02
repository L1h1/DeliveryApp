using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.Product.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _productRepository;
        private readonly IDistributedCache _distributedCache;

        public DeleteProductCommandHandler(IProductRepository productRepository, IDistributedCache distributedCache)
        {
            _productRepository = productRepository;
            _distributedCache = distributedCache;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingProduct is null)
            {
                throw new NotFoundException("Prodct with given id not found.");
            }

            await _productRepository.DeleteAsync(existingProduct, cancellationToken);
            await _distributedCache.RemoveAsync($"product:{request.Id}", cancellationToken);

            return Unit.Value;
        }
    }
}
