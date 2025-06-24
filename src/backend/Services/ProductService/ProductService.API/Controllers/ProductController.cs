using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] PageRequestDTO page, CancellationToken cancellationToken)
        {
            var query = new GetAllProductsQuery(page);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] FilterProductsRequestDTO dto, CancellationToken cancellationToken)
        {
            var query = new GetFilteredProductsQuery(dto);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery(id);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new CreateProductCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPost("details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProductDetails([FromBody] ProductDetailsRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new CreateProductDetailsCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] ProductRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new UpdateProductCommand(id, dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPut("details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductDetails([FromBody] ProductDetailsRequestDTO dto, CancellationToken cancellationToken)
        {
            var command = new UpdateProductDetailsCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand(id);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Product deleted successfully." });
        }

        [HttpDelete("details/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductDetails([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductDetailsCommand(id);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Product details deleted successfully." });
        }
    }
}
