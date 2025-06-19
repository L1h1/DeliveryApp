using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.ProductDetails.DeleteProductDetails
{
    public class DeleteProductDetailsCommandHandler : IRequestHandler<DeleteProductDetailsCommand, Unit>
    {
        private readonly IProductDetailsRepository _productDetailsRepository;
        private readonly IDistributedCache _distributedCache;

        public DeleteProductDetailsCommandHandler(IProductDetailsRepository productDetailsRepository, IDistributedCache distributedCache)
        {
            _productDetailsRepository = productDetailsRepository;
            _distributedCache = distributedCache;
        }

        public async Task<Unit> Handle(DeleteProductDetailsCommand request, CancellationToken cancellationToken)
        {
            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingDetails is null)
            {
                throw new NotFoundException("Details for given id not found.");
            }

            await _productDetailsRepository.DeleteAsync(existingDetails, cancellationToken);
            await _distributedCache.RemoveAsync($"product:{request.Id}", cancellationToken);

            return Unit.Value;
        }
    }
}
