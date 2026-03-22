using GeniusStoreERP.Domain.Common;
using GeniusStoreERP.Domain.Entities.Stock;

namespace GeniusStoreERP.Domain.Entities.Transactions;

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal DisCountAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetLineTotal { get; set; }

    //navigation properties
    public Invoice? Invoice { get; set; }
    public Product? Product { get; set; }
}