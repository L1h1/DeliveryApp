using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.AssignCourier;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Commands.DeleteOrder;
using OrderService.Application.Commands.UpdateOrderStatus;
using OrderService.Application.DTOs.Request;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Application.Queries.GetOrdersByClientId;
using OrderService.Application.Queries.GetOrdersByCourierId;
using OrderService.Application.Queries.GetOrdersByStatus;
using OrderService.Domain.Enums;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            var query = new GetOrderByIdQuery(orderId);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("client/{clientId}")]
        [Authorize(Roles = "Admin, Courier")]
        public async Task<IActionResult> GetOrdersByClientId([FromRoute] Guid clientId, CancellationToken cancellationToken)
        {
            var query = new GetOrdersByClientIdQuery(clientId);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("courier/{courierId}")]
        [Authorize(Roles = "Admin, Courier")]
        public async Task<IActionResult> GetActiveOrdersByCourierId([FromRoute] Guid courierId, CancellationToken cancellationToken)
        {
            var query = new GetOrdersByCourierIdQuery(courierId);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByStatus([FromRoute] OrderStatus status, CancellationToken cancellationToken)
        {
            var query = new GetOrdersByStatusQuery(status);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO requestDTO, CancellationToken cancellationToken)
        {
            var command = new CreateOrderCommand(requestDTO);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPatch("orders/{orderId}/courier")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignCourierAsync([FromRoute] Guid orderId, [FromBody] Guid courierId, CancellationToken cancellationToken)
        {
            var command = new AssignCourierCommand(orderId, courierId);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Courier assigned." });
        }

        [HttpPatch("orders/{orderId}/status")]
        [Authorize(Roles = "Admin, Courier")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromBody] OrderStatus status, CancellationToken cancellationToken)
        {
            var command = new UpdateOrderStatusCommand(orderId, status);
            await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Status updated." });
        }

        [HttpDelete("{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderCommand([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            var command = new DeleteOrderCommand(orderId);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { Message = "Order deleted." });
        }
    }
}
