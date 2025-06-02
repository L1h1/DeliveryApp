namespace ProductService.Application.DTOs.Request
{
    public sealed record CategoryRequestDTO
    {
        required public string Name { get; init; }
    }
}
