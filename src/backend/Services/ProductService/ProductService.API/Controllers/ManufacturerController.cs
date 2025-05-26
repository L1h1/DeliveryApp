using MediatR;
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
    public class ManufacturerController : Controller
    {
        private readonly IMediator _mediator;

        public ManufacturerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllManufacturers([FromQuery] PageRequestDTO page, CancellationToken cancellationToken)
        {
            var query = new GetAllManufacturersQuery(page);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateManufacturer([FromBody] ManufacturerRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new CreateManufacturerCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateManufacturer([FromRoute] Guid id, [FromBody] ManufacturerRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new UpdateManufacturerCommand(id, dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManufacturer([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteManufacturerCommand(id);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Manufacturer deleted successfully." });
        }
    }
}
