using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Data.SQL.EntityConfigurations
{
    public class ManufacturerEntityConfiguration : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasIndex(m => m.NormalizedName).IsUnique();

            builder.Property(m => m.Country).HasMaxLength(64).IsRequired();
            builder.Property(m => m.Name).HasMaxLength(128).IsRequired();
            builder.Property(m => m.NormalizedName).HasMaxLength(128).IsRequired();
            builder.Property(m => m.Address).HasMaxLength(256).IsRequired();
        }
    }
}
