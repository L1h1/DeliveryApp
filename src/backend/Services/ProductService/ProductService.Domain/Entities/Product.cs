using ProductService.Domain.Enums;

namespace ProductService.Domain.Entities
{
    public class Product
    {
        required public Guid Id { get; set; }
        required public string Title { get; set; }
        required public decimal Price { get; set; }
        required public UnitOfMeasure UnitOfMeasure { get; set; }
        required public bool IsAvailable { get; set; }
        public string? Thumbnail { get; set; }
        required public Guid ManufacturerId { get; set; }
        required public Manufacturer Manufacturer { get; set; }
        public List<Category> Categories { get; set; } = new ();
    }
}
