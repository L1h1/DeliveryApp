using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Queries.Category.GetAllCategories;
using ProductService.Application.Queries.Product.GetFilteredProducts;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : Controller
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> TestPoint()
        {
            var result = await _mediator.Send(new GetFilteredProductsQuery(new Application.DTOs.Request.FilterProductsRequestDTO()
            {
                Page = new Application.DTOs.Request.PageRequestDTO { 
                    PageNumber = 1,
                    PageSize = 1,
                },
            }));
            return Ok(result);
        }
    }
}
