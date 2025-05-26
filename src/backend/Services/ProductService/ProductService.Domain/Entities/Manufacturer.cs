namespace ProductService.Domain.Entities
{
    public class Manufacturer
    {
        required public Guid Id { get; set; }
        required public string Name { get; set; }
        required public string NormalizedName { get; set; }
        required public string Country { get; set; }
        required public string Address { get; set; }
        public List<Product> Products { get; set; } = new ();
    }
}
