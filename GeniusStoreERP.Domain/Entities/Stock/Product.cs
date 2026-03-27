using GeniusStoreERP.Domain.Common;
using GeniusStoreERP.Domain.Entities.Transactions;

namespace GeniusStoreERP.Domain.Entities.Stock;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal StockQuantity { get; set; }
    public decimal ReorderLevel { get; set; }
    public string? SKU { get; set; }
    public string? Barcode { get; set; }
    public int CategoryId { get; set; }

    // Navigation Property
    public Category? Category { get; set; }
    public ICollection<InvoiceItem>? InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<StockTransaction>? StockTransactions { get; set; } = new List<StockTransaction>();
}