using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Invoice> builder)
    {
        // Relationships
        builder.HasOne(i => i.Partner)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.InvoiceItems)
                .WithOne(ii => ii.Invoice)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(i => new { i.InvoiceNumber, i.InvoiceTypeId }).IsUnique();
        builder.HasIndex(i => i.InvoiceDate);

        // Properties
        builder.Property(i => i.InvoiceNumber)
               .IsRequired();
        builder.Property(i => i.InvoiceDate)
               .IsRequired();
        builder.Property(i => i.TotalItemsAmount)
               .HasPrecision(18, 2);
        builder.Property(i => i.TotalItemsDiscount)
               .HasPrecision(18, 2);
        builder.Property(i => i.TotalItemsTax)
               .HasPrecision(18, 2);
        builder.Property(i => i.FinalAmount)
               .HasPrecision(18, 2);
        builder.Property(i => i.Notes)
               .HasMaxLength(500);
    }
}
