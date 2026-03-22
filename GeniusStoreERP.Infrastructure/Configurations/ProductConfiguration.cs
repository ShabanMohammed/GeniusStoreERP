using GeniusStoreERP.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.Name).IsUnique();
            builder.HasIndex(p => p.Barcode).IsUnique().HasFilter("\"Barcode\" IS NOT NUll");
            builder.HasIndex(p => p.SKU).IsUnique().HasFilter("\"SKU\" IS NOT NUll");

            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            // ضبط دقة حقل رصيد المخزن
            builder.Property(p => p.StockQuantity)
                   .HasPrecision(18, 4);

            // ضبط دقة حقل حد الطلب بنفس الطريقة
            builder.Property(p => p.ReorderLevel)
                   .HasPrecision(18, 4);

            // ضبط دقة السعر (عادة خانتين عشريتين كافية للعملات)
            builder.Property(p => p.Price)
                   .HasPrecision(18, 2);

            builder.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
