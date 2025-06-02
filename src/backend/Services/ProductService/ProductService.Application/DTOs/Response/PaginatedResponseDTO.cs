namespace ProductService.Application.DTOs.Response
{
    public sealed record PaginatedResponseDTO<T>
    {
        required public ICollection<T> Items { get; init; }
        required public long TotalCount { get; init; }
        required public int PageSize { get; init; }
        required public int PageNumber { get; set; }
        public long PageCount => (long)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
