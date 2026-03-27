using GeniusStoreERP.Domain.Entities;
using GeniusStoreERP.Domain.Entities.Partners;
using GeniusStoreERP.Domain.Entities.Stock;
using GeniusStoreERP.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<InvoiceType> InvoiceTypes { get; set; }
        public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
        public DbSet<GeneralSetting> GeneralSettings { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<StockTransactionType> TransactionTypes { get; set; }


        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
