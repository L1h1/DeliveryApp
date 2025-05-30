using MediatR;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Commands.Images.AddProductThumbnail
{
    public class AddProductThumbnailCommandHandler : IRequestHandler<AddProductThumbnailCommand, string>
    {
        private readonly IImageStorageService _imageStorageService;
        private readonly IProductRepository _productRepository;

        public AddProductThumbnailCommandHandler(IImageStorageService imageStorageService, IProductRepository productRepository)
        {
            _imageStorageService = imageStorageService;
            _productRepository = productRepository;
        }

        public async Task<string> Handle(AddProductThumbnailCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            var imagePath = await _imageStorageService.SaveThumbnailAsync(request.ProductId, request.File, cancellationToken);

            product.Thumbnail = imagePath;

            await _productRepository.UpdateAsync(product, cancellationToken);

            return imagePath;
        }
    }
}
