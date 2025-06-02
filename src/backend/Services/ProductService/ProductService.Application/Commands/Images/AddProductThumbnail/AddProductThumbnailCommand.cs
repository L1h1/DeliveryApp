using MediatR;
using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Commands.Images.AddProductThumbnail
{
    public sealed record AddProductThumbnailCommand(Guid ProductId, IFormFile File) : IRequest<string>
    {
    }
}
