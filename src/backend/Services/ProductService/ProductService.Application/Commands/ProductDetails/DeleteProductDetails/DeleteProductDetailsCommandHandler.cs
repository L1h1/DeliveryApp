using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.ProductDetails.DeleteProductDetails
{
    public class DeleteProductDetailsCommandHandler : IRequestHandler<DeleteProductDetailsCommand, Unit>
    {
        private readonly IProductDetailsRepository _productDetailsRepository;
        private readonly ILogger<DeleteProductDetailsCommandHandler> _logger;

        public DeleteProductDetailsCommandHandler(IProductDetailsRepository productDetailsRepository, ILogger<DeleteProductDetailsCommandHandler> logger)
        {
            _productDetailsRepository = productDetailsRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteProductDetailsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting product @{id}", request.Id);

            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingDetails is null)
            {
                throw new NotFoundException("Details for given id not found.");
            }

            await _productDetailsRepository.DeleteAsync(existingDetails, cancellationToken);

            _logger.LogInformation("Successfully deleted product @{id}", request.Id);

            return Unit.Value;
        }
    }
}
