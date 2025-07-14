using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.Manufacturer.CreateManufacturer;
using ProductService.Application.Commands.Manufacturer.DeleteManufacturer;
using ProductService.Application.Commands.Manufacturer.UpdateManufacturer;
using ProductService.Application.DTOs.Request;
using ProductService.Application.Queries.Manufacturer.GetAllManufacturers;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/manufacturers")]
    public class ManufacturerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ManufacturerController> _logger;

        public ManufacturerController(IMediator mediator, ILogger<ManufacturerController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllManufacturers([FromQuery] PageRequestDTO page, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve manufacturers page {@page}", page);

            var query = new GetAllManufacturersQuery(page);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved manufacturers page {@page}", page);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateManufacturer([FromBody] ManufacturerRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create manufacturer {@manufacturer}", dto);

            var command = new CreateManufacturerCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully created manufacturer {@manufacturer}", dto);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateManufacturer([FromRoute] Guid id, [FromBody] ManufacturerRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update @{id} manufacturer {@manufacturer}", id, dto);

            var command = new UpdateManufacturerCommand(id, dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully updated @{id} manufacturer {@manufacturer}", id, dto);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteManufacturer([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete manufacturer @{id}", id);

            var command = new DeleteManufacturerCommand(id);
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully deleted manufacturer @{id}", id);

            return Ok(new { Message = "Manufacturer deleted successfully." });
        }
    }
}
