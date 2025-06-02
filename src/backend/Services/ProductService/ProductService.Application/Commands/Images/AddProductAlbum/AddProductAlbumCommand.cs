using MediatR;
using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Commands.Images.AddProductAlbum
{
    public sealed record AddProductAlbumCommand(Guid ProductId, IEnumerable<IFormFile> Files) : IRequest<List<string>>
    {
    }
}
