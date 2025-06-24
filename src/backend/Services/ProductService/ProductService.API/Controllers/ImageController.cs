using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.Images.AddProductAlbum;
using ProductService.Application.Commands.Images.AddProductThumbnail;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/images")]
    [Authorize(Roles = "Admin")]
    public class ImageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("album/{productId}")]
        public async Task<IActionResult> SaveAlbum([FromRoute] Guid productId, IEnumerable<IFormFile> files, CancellationToken cancellationToken)
        {
            var command = new AddProductAlbumCommand(productId, files);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPost("thumbnail/{productId}")]
        public async Task<IActionResult> SaveThumbnail([FromRoute] Guid productId, IFormFile file, CancellationToken cancellationToken)
        {
            var command = new AddProductThumbnailCommand(productId, file);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }
    }
}
