using System;
using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasOne(i => i.Partner)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PartnerId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.InvoiceType)
                .WithMany(it => it.Invoices)
                .HasForeignKey(i => i.InvoiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.InvoiceStatus)
                .WithMany(s => s.Invoices)
                .HasForeignKey(i => i.InvoiceStatusId)
                .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.InvoiceNumber, i.InvoiceTypeId }).IsUnique();
        builder.Property(i => i.InvoiceNumber)
               .IsRequired()
               .HasMaxLength(50);
        builder.Property(i => i.InvoiceDate).IsRequired();
        builder.Property(i => i.TotalItemsAmount).HasPrecision(18, 2);
        builder.Property(i => i.TotalItemsDiscount).HasPrecision(18, 2);
        builder.Property(i => i.TotalItemsTax).HasPrecision(18, 2);
        builder.Property(i => i.FinalAmount).HasPrecision(18, 2);

        builder.Property(i => i.Notes)
               .HasMaxLength(500);

    }
}
