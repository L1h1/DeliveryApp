using System.Text.Json.Serialization;

namespace ProductService.Domain.Entities
{
    public class Category : BaseEntity
    {
        required public string Name { get; set; }
        required public string NormalizedName { get; set; }
        [JsonIgnore]
        public List<Product> Products { get; set; } = new ();
    }
}
