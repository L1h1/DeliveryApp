using MediatR;
using ProductService.Application.Exceptions;
using ProductService.Application.Interfaces.Repositories;

namespace ProductService.Application.Commands.ProductDetails.DeleteProductDetails
{
    public class DeleteProductDetailsCommandHandler : IRequestHandler<DeleteProductDetailsCommand, Unit>
    {
        private readonly IProductDetailsRepository _productDetailsRepository;

        public DeleteProductDetailsCommandHandler(IProductDetailsRepository productDetailsRepository)
        {
            _productDetailsRepository = productDetailsRepository;
        }

        public async Task<Unit> Handle(DeleteProductDetailsCommand request, CancellationToken cancellationToken)
        {
            var existingDetails = await _productDetailsRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingDetails is null)
            {
                throw new NotFoundException("Details for given id not found.");
            }

            await _productDetailsRepository.DeleteAsync(existingDetails, cancellationToken);

            return Unit.Value;
        }
    }
}
