using GeniusStoreERP.Domain.Common;
using GeniusStoreERP.Domain.Entities.Transactions;

namespace GeniusStoreERP.Domain.Entities.Stock;

public class StockTransaction : BaseEntity
{

    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public int StockTransactionTypeId { get; set; }
    public DateTime TransactionDate { get; set; }
    public int? InvoiceId { get; set; }
    public string? InvoiceReference { get; set; }
    public int? AdjustmentId { get; set; }
    public string? AdjustmentReference { get; set; }
    public string? Remarks { get; set; }

    //navtion property
    public Product? Product { get; set; }
    public Invoice? Invoice { get; set; }
    public StockTransactionType? Type { get; set; }

}
