using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.AssignCourier;
using OrderService.Application.Interfaces.Services;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IMediator _mediator;

        public TestController(IUserService userService, IMediator mediator, IProductService productService)
        {
            _userService = userService;
            _mediator = mediator;
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<Guid> ids, CancellationToken cancellationToken)
        {
            var result = await _productService.GetProductsAsync(ids, cancellationToken);

            return Ok(result);
        }
    }
}
