namespace GeniusStoreERP.Infrastructure.Configurations;

using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InvoiceStatusConfiguration : IEntityTypeConfiguration<InvoiceStatus>
{
    public void Configure(EntityTypeBuilder<InvoiceStatus> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(s => s.Name)
          .IsUnique();

        builder.Property(s => s.Name)
               .IsRequired()
               .HasMaxLength(50);
        builder.HasMany(s => s.Invoices)
               .WithOne(i => i.InvoiceStatus)
               .HasForeignKey(i => i.InvoiceStatusId)
               .OnDelete(DeleteBehavior.Restrict);
        // Seed Data لجدول طرق السداد
        builder.HasData(
            new InvoiceStatus { Id = 1, Name = "نشط" },
            new InvoiceStatus { Id = 2, Name = "ملغاة" }

        );

    }
}
