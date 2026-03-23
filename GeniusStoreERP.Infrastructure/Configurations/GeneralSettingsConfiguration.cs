using GeniusStoreERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeniusStoreERP.Infrastructure.Data.Configurations;

public class GeneralSettingsConfiguration : IEntityTypeConfiguration<GeneralSettings>
{
    public void Configure(EntityTypeBuilder<GeneralSettings> builder)
    {


        // تحديد المفتاح الأساسي (رغم أنه موروث من BaseEntity)
        builder.HasKey(x => x.Id);

        // إعدادات الحقول النصية (مطلوبة)
        builder.Property(x => x.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CurrencySymbol)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("EGP");

        // إعدادات الحقول الاختيارية مع تحديد الطول
        builder.Property(x => x.LegalName).HasMaxLength(250);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Email).HasMaxLength(150);
        builder.Property(x => x.Website).HasMaxLength(150);
        builder.Property(x => x.TaxNumber).HasMaxLength(50);

        // أهم جزء: إعدادات الحقول المالية (الضريبة)
        // تحديد الدقة (Precision) لضمان عدم حدوث تقريب في PostgreSQL
        builder.Property(x => x.TaxPercentage)
            .HasColumnType("numeric(5,2)") // يسمح بـ 999.99 كحد أقصى
            .HasDefaultValue(14.00);

        // إعداد اللوجو كـ Binary Large Object (BLOB)
        // في Postgres سيتحول تلقائياً لـ bytea
        builder.Property(x => x.Logo)
            .IsRequired(false);
    }
}