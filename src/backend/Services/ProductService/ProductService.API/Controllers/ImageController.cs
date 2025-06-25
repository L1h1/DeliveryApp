using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.Images.AddProductAlbum;
using ProductService.Application.Commands.Images.AddProductThumbnail;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ImageController> _logger;

        public ImageController(IMediator mediator, ILogger<ImageController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("album/{productId}")]
        public async Task<IActionResult> SaveAlbum([FromRoute] Guid productId, IEnumerable<IFormFile> files, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to save album images for product @{productId}", productId);

            var command = new AddProductAlbumCommand(productId, files);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully saved album images for product @{productId}", productId);

            return Ok(response);
        }

        [HttpPost("thumbnail/{productId}")]
        public async Task<IActionResult> SaveThumbnail([FromRoute] Guid productId, IFormFile file, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to save thumbnail for product @{productId}", productId);

            var command = new AddProductThumbnailCommand(productId, file);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully saved thumbnail for product @{productId}", productId);

            return Ok(response);
        }
    }
}
