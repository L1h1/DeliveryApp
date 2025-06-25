using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Commands.Images.AddProductAlbum
{
    public class AddProductAlbumCommandHandler : IRequestHandler<AddProductAlbumCommand, List<string>>
    {
        private readonly IImageStorageService _imageStorageService;
        private readonly IProductDetailsRepository _productDetailsRepository;
        private readonly ILogger<AddProductAlbumCommandHandler> _logger;

        public AddProductAlbumCommandHandler(
            IImageStorageService imageStorageService,
            IProductDetailsRepository productDetailsRepository,
            ILogger<AddProductAlbumCommandHandler> logger)
        {
            _imageStorageService = imageStorageService;
            _productDetailsRepository = productDetailsRepository;
            _logger = logger;
        }

        public async Task<List<string>> Handle(AddProductAlbumCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saving album for product @{id}", request.ProductId);

            var productDetails = await _productDetailsRepository.GetByIdAsync(request.ProductId, cancellationToken);

            _imageStorageService.DeleteImages(productDetails.Images!);

            var imagePaths = await _imageStorageService.SaveAlbumAsync(request.ProductId, request.Files, cancellationToken);

            productDetails.Images = imagePaths;
            await _productDetailsRepository.UpdateAsync(productDetails, cancellationToken);

            _logger.LogInformation("Successfully saved album for product @{id}", request.ProductId);

            return imagePaths;
        }
    }
}
