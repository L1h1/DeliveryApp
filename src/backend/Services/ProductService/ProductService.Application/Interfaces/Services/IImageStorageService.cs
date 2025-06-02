using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Interfaces.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveThumbnailAsync(Guid productId, IFormFile file, CancellationToken cancellationToken = default);
        Task<List<string>> SaveAlbumAsync(Guid productId, IEnumerable<IFormFile> files, CancellationToken cancellationToken = default);
        void DeleteImages(List<string> paths);
    }
}
