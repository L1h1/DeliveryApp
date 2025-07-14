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
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve order @{id}", orderId);

            var query = new GetOrderByIdQuery(orderId);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved order @{id}", orderId);

            return Ok(response);
        }

        [HttpGet("client/{clientId}")]
        [Authorize(Roles = "Admin, Courier")]
        public async Task<IActionResult> GetOrdersByClientId([FromRoute] Guid clientId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve orders for client @{id}", clientId);

            var query = new GetOrdersByClientIdQuery(clientId);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved orders for client @{id}", clientId);

            return Ok(response);
        }

        [HttpGet("courier/{courierId}")]
        [Authorize(Roles = "Admin, Courier")]
        public async Task<IActionResult> GetActiveOrdersByCourierId([FromRoute] Guid courierId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve orders for courier @{id}", courierId);

            var query = new GetOrdersByCourierIdQuery(courierId);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved orders for courier @{id}", courierId);

            return Ok(response);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByStatus([FromRoute] OrderStatus status, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to get orders with @{status}", status);

            var query = new GetOrdersByStatusQuery(status);
            var response = await _mediator.Send(query, cancellationToken);

            _logger.LogInformation("Successfully retrieved orders with @{status}", status);

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO requestDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create order for client @{id}", requestDTO.ClientId);

            var command = new CreateOrderCommand(requestDTO);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully created order @{id} for client @{id}", response.Id, requestDTO.ClientId);

            return Ok(response);
        }

        [HttpPatch("orders/{orderId}/courier")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignCourierAsync([FromRoute] Guid orderId, [FromBody] Guid courierId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to assign courier @{id} to order @{id}", courierId, orderId);

            var command = new AssignCourierCommand(orderId, courierId);
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully assigned courier @{id} to order @{id}", courierId, orderId);

            return Ok(new { Message = "Courier assigned." });
        }

        [HttpPatch("orders/{orderId}/status")]
        [Authorize(Roles = "Admin, Courier")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromBody] OrderStatus status, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update order @{id} with status @{status}", orderId, status);

            var command = new UpdateOrderStatusCommand(orderId, status);
            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully updated order @{id} with status @{status}", orderId, status);

            return Ok(new { Message = "Status updated." });
        }

        [HttpDelete("{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderCommand([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete order @{id}", orderId);

            var command = new DeleteOrderCommand(orderId);
            var response = await _mediator.Send(command, cancellationToken);

            _logger.LogInformation("Successfully deleted order @{id}", orderId);

            return Ok(new { Message = "Order deleted." });
        }
    }
}
