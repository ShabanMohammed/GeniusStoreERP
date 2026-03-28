using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasIndex(p => p.Name).IsUnique();
            builder.HasIndex(p => p.Barcode).IsUnique()
                   .HasFilter("\"Barcode\" IS NOT NUll AND \"Barcode\" != ''");
            builder.HasIndex(p => p.SKU).IsUnique()
                    .HasFilter("\"SKU\" IS NOT NUll AND \"SKU\" != ''");

            builder.Property(p => p.Name).IsRequired()
                   .HasMaxLength(100);
            builder.Property(p => p.Price)
                  .HasPrecision(18, 2);
            builder.Property(p => p.StockQuantity)
                   .HasPrecision(18, 4);
            builder.Property(p => p.ReorderLevel)
                   .HasPrecision(18, 4);
            builder.Property(p => p.Barcode)
                   .HasMaxLength(50);
            builder.Property(p => p.SKU)
                   .HasMaxLength(50);

            builder.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
