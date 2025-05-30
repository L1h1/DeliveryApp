namespace ProductService.Application.DTOs.Response
{
    public sealed record class CategoryResponseDTO
    {
        required public Guid Id { get; init; }
        required public string Name { get; init; }
    }
}
