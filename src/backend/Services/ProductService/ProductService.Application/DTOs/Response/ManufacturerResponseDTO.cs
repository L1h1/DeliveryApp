namespace ProductService.Application.DTOs.Response
{
    public sealed record ManufacturerResponseDTO
    {
        required public Guid Id { get; init; }
        required public string Name { get; init; }
        required public string Country { get; init; }
        required public string Address { get; init; }
    }
}
