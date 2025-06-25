using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Commands.Images.AddProductThumbnail
{
    public class AddProductThumbnailCommandHandler : IRequestHandler<AddProductThumbnailCommand, string>
    {
        private readonly IImageStorageService _imageStorageService;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<AddProductThumbnailCommandHandler> _logger;

        public AddProductThumbnailCommandHandler(
            IImageStorageService imageStorageService,
            IProductRepository productRepository,
            ILogger<AddProductThumbnailCommandHandler> logger)
        {
            _imageStorageService = imageStorageService;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<string> Handle(AddProductThumbnailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saving thumbnail for product @{id}", request.ProductId);

            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            var imagePath = await _imageStorageService.SaveThumbnailAsync(request.ProductId, request.File, cancellationToken);

            product.Thumbnail = imagePath;

            await _productRepository.UpdateAsync(product, cancellationToken);

            _logger.LogInformation("Successfully saved thumbnail for product @{id}", request.ProductId);

            return imagePath;
        }
    }
}
