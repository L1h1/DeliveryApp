using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.Category.CreateCategory;
using ProductService.Application.Commands.Category.DeleteCategory;
using ProductService.Application.Commands.Category.UpdateCategory;
using ProductService.Application.DTOs.Request;
using ProductService.Application.Queries.Category.GetAllCategories;
using ProductService.Application.Queries.Category.GetCategoryByName;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] PageRequestDTO page, CancellationToken cancellationToken)
        {
            var query = new GetAllCategoriesQuery(page);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetCategoryByName([FromRoute] string name, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByNameQuery(name);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new CreateCategoryCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] CategoryRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new UpdateCategoryCommand(id, dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Category deleted successfully." });
        }
    }
}