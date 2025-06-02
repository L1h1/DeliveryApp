namespace ProductService.Application.DTOs.Request
{
    public sealed record ManufacturerRequestDTO
    {
        required public string Name { get; init; }
        required public string Country { get; init; }
        required public string Address { get; init; }
    }
}
