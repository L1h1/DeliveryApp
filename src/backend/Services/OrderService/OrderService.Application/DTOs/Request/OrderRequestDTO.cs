namespace OrderService.Application.DTOs.Request
{
    public sealed record OrderRequestDTO
    {
        required public Guid ClientId { get; init; }
        required public List<OrderItemRequestDTO> Items { get; init; }
        required public string Address { get; init; }
        public string? ClientComment { get; init; }
    }
}
