using System.Text.Json.Serialization;

namespace ProductService.Domain.Entities
{
    public class Manufacturer : BaseEntity
    {
        required public string Name { get; set; }
        required public string NormalizedName { get; set; }
        required public string Country { get; set; }
        required public string Address { get; set; }
        [JsonIgnore]
        public List<Product> Products { get; set; } = new ();
    }
}
