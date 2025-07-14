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
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IMediator mediator, ILogger<CategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] PageRequestDTO page, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve category page {@page}", page);

            var query = new GetAllCategoriesQuery(page);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved category page {@page}", page);

            return Ok(response);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetCategoryByName([FromRoute] string name, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve category data by @{name}", name);

            var query = new GetCategoryByNameQuery(name);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved category data by @{name}", name);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create category with @{name}", dto.Name);

            var command = new CreateCategoryCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully created category with @{name}", dto.Name);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] CategoryRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update category with @{name}", dto.Name);

            var command = new UpdateCategoryCommand(id, dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully updated category with @{name}", dto.Name);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete category with  @{id}", id);

            var command = new DeleteCategoryCommand(id);
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully deleted category with  @{id}", id);

            return Ok(new { Message = "Category deleted successfully." });
        }
    }
}