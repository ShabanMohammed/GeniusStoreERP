namespace GeniusStoreERP.Domain.Entities.Stock;

public class StockTransaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; } // موجبة للإضافة، سالبة للصرف
    public int TransactionType { get; set; } // (1: فاتورة، 2: تسوية جردية، 3: هالك)
    public DateTime TransactionDate { get; set; }

    // الربط الاختياري (Nullable)
    public int? InvoiceId { get; set; } // إذا كانت ناتجة عن فاتورة
    public int? AdjustmentId { get; set; } // إذا كانت ناتجة عن أمر تسوية منفصل

}
