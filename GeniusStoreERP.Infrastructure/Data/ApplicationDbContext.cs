using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities;
using GeniusStoreERP.Domain.Entities.Partners;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace GeniusStoreERP.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Partner> Partners { get; set; }

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<InvoiceType> InvoiceTypes { get; set; }
    public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
    public DbSet<GeneralSetting> GeneralSettings { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }


    public new DbSet<ApplicationUser> Users { get; set; }


    ///
    private IDbContextTransaction? _currenttransaction;
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currenttransaction = await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currenttransaction == null) return;
        try
        {

            if (_currenttransaction != null)
                await _currenttransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            if (_currenttransaction != null)
                await _currenttransaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {

            _currenttransaction?.Dispose();
            _currenttransaction = null;

        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currenttransaction != null)
                await _currenttransaction.RollbackAsync(cancellationToken);
        }
        finally
        {

            _currenttransaction?.Dispose();
            _currenttransaction = null;


        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Seed Data لجدول أنواع الفواتير
        builder.Entity<InvoiceType>().HasData(
            new InvoiceType { Id = 1, Name = "مبيعات" },
            new InvoiceType { Id = 2, Name = "مشتريات" },
            new InvoiceType { Id = 3, Name = "مرتجع مبيعات" },
            new InvoiceType { Id = 4, Name = "مرتجع مشتريات" }
        );

        // Seed Data لجدول طرق السداد
        builder.Entity<InvoiceStatus>().HasData(
            new InvoiceStatus { Id = 1, Name = "نقدي" },
            new InvoiceStatus { Id = 2, Name = "آجل" }

        );

        builder.Entity<TransactionType>().HasData(
            new TransactionType { Id = 1, Name = "فاتورة" },
            new TransactionType { Id = 2, Name = "تسوية" },
            new TransactionType { Id = 3, Name = "تلف" }

            );


    }

}
