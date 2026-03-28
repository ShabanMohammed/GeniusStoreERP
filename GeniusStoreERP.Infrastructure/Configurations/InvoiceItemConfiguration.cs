using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasIndex(ii => ii.InvoiceId);
        builder.HasIndex(ii => ii.ProductId);

        builder.Property(ii => ii.Quantity)
               .HasPrecision(18, 4)
               .IsRequired();
        builder.Property(ii => ii.UnitPrice)
               .HasPrecision(18, 2)
               .IsRequired();
        builder.Property(ii => ii.DiscountRate)
               .HasPrecision(18, 2);
        builder.Property(ii => ii.DisCountAmount)
               .HasPrecision(18, 2);
        builder.Property(ii => ii.TaxRate)
               .HasPrecision(18, 2);
        builder.Property(ii => ii.TaxAmount)
               .HasPrecision(18, 2);
        builder.Property(ii => ii.NetLineTotal)
               .HasPrecision(18, 2);

    }
}