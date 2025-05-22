namespace ProductService.Application.DTOs.Request
{
    public sealed record PageRequestDTO
    {
        required public int PageNumber { get; init; }
        required public int PageSize { get; init; }
    }
}
