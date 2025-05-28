namespace OrderService.Application.DTOs.Request
{
    public sealed record OrderItemRequestDTO
    {
        required public Guid ProductId { get; init; }
        required public int Quantity { get; init; }
    }
}
