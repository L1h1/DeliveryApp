using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProductService.Application.Commands.Product.CreateProduct;
using ProductService.Application.Commands.Product.DeleteProduct;
using ProductService.Application.Commands.Product.UpdateProduct;
using ProductService.Application.Commands.ProductDetails.CreateProductDetails;
using ProductService.Application.Commands.ProductDetails.DeleteProductDetails;
using ProductService.Application.Commands.ProductDetails.UpdateProductDetails;
using ProductService.Application.DTOs.Request;
using ProductService.Application.Queries.Product.GetAllProducts;
using ProductService.Application.Queries.Product.GetFilteredProducts;
using ProductService.Application.Queries.Product.GetProductById;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMediator mediator, ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PageRequestDTO page, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to get product page {@page}", page);

            var query = new GetAllProductsQuery(page);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved product page {@page}", page);

            return Ok(response);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] FilterProductsRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to get filtered product page {@page}", dto.Page);

            var query = new GetFilteredProductsQuery(dto);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved filtered product page {@page}", dto.Page);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to get product by id @{id}", id);

            var query = new GetProductByIdQuery(id);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved product by id @{id}", id);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create product {@product}", dto);

            var command = new CreateProductCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully created product {@product}", dto);

            return Ok(response);
        }

        [HttpPost("details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductDetails([FromBody] ProductDetailsRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add details to product {@id}", dto.ProductId);

            var command = new CreateProductDetailsCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully added details to product {@id}", dto.ProductId);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] ProductRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update @{id} product", id);

            var command = new UpdateProductCommand(id, dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully updated @{id} product", id);

            return Ok(response);
        }

        [HttpPut("details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductDetails([FromBody] ProductDetailsRequestDTO dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update details for product @{id}", dto.ProductId);

            var command = new UpdateProductDetailsCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully updated details for product @{id}", dto.ProductId);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete product @{id}", id);

            var command = new DeleteProductCommand(id);
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully deleted product @{id}", id);

            return Ok(new { Message = "Product deleted successfully." });
        }

        [HttpDelete("details/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductDetails([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete details for product @{id}", id);

            var command = new DeleteProductDetailsCommand(id);
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully deleted details for product @{id}", id);

            return Ok(new { Message = "Product details deleted successfully." });
        }
    }
}
