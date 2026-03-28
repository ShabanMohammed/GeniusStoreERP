using GeniusStoreERP.Domain.Entities.Finances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Configurations;

public class PartnerTransactionConfiguration : IEntityTypeConfiguration<PartnerTransaction>
{
    public void Configure(EntityTypeBuilder<PartnerTransaction> builder)
    {
        builder.Property(p => p.Debit)
            .HasPrecision(18, 2);

        builder.Property(p => p.Credit)
            .HasPrecision(18, 2);

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(50);

        builder.Property(p => p.Remarks)
            .HasMaxLength(500);

        builder.HasOne(p => p.Partner)
            .WithMany()
            .HasForeignKey(p => p.PartnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Invoice)
            .WithMany()
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Type)
            .WithMany()
            .HasForeignKey("TransactionTypeId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.PartnerId);
        builder.HasIndex(p => p.TransactionDate);
    }
}
