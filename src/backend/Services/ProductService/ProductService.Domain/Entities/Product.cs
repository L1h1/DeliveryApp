using ProductService.Domain.Enums;

namespace ProductService.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public bool IsAvailable { get; set; }
        public Guid ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public List<Category> Categories { get; set; } = new ();
    }
}
