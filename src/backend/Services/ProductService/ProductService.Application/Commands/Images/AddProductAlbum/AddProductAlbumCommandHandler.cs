using MediatR;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Commands.Images.AddProductAlbum
{
    public class AddProductAlbumCommandHandler : IRequestHandler<AddProductAlbumCommand, List<string>>
    {
        private readonly IImageStorageService _imageStorageService;
        private readonly IProductDetailsRepository _productDetailsRepository;

        public AddProductAlbumCommandHandler(IImageStorageService imageStorageService, IProductDetailsRepository productDetailsRepository)
        {
            _imageStorageService = imageStorageService;
            _productDetailsRepository = productDetailsRepository;
        }

        public async Task<List<string>> Handle(AddProductAlbumCommand request, CancellationToken cancellationToken)
        {
            var productDetails = await _productDetailsRepository.GetByIdAsync(request.ProductId, cancellationToken);

            _imageStorageService.DeleteImages(productDetails.Images!);

            var imagePaths = await _imageStorageService.SaveAlbumAsync(request.ProductId, request.Files, cancellationToken);

            productDetails.Images = imagePaths;
            await _productDetailsRepository.UpdateAsync(productDetails, cancellationToken);

            return imagePaths;
        }
    }
}
