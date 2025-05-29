using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Application.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly string rootPath;

        public ImageStorageService(IConfiguration configuration)
        {
            rootPath = configuration.GetValue<string>("ImageFolder")!;
        }

        public async Task<string> SaveThumbnailAsync(Guid productId, IFormFile file, CancellationToken cancellationToken = default)
        {
            var productFolder = Path.Combine(rootPath, "Products", productId.ToString());
            Directory.CreateDirectory(productFolder);

            var extension = Path.GetExtension(file.FileName);
            var thumbnailPath = Path.Combine(productFolder, $"{productId}{extension}");

            if (File.Exists(thumbnailPath))
            {
                File.Delete(thumbnailPath);
            }

            using var stream = new FileStream(thumbnailPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);

            return thumbnailPath;
        }

        public async Task<List<string>> SaveAlbumAsync(Guid productId, IEnumerable<IFormFile> files, CancellationToken cancellationToken = default)
        {
            var productFolder = Path.Combine(rootPath, "Products", productId.ToString());
            Directory.CreateDirectory(productFolder);

            var semaphoreSlim = new SemaphoreSlim(4);

            var tasks = files.Select(async file =>
            {
                if (file.Length == 0)
                {
                    return null;
                }

                await semaphoreSlim.WaitAsync(cancellationToken);

                try
                {
                    var extension = Path.GetExtension(file.FileName);
                    var uniqueName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(productFolder, uniqueName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream, cancellationToken);
                    return filePath;
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            });

            var paths = await Task.WhenAll(tasks);

            return paths.Where(path => path != null).ToList();
        }

        public void DeleteImages(List<string> paths)
        {
            foreach (var path in paths)
            {
                File.Delete(path);
            }
        }
    }
}
