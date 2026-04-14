using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities.Stock;

public class StockAdjustmentItem : BaseEntity
{
    public int StockAdjustmentId { get; set; }
    public int ProductId { get; set; }
    
    // The previous stock quantity before this adjustment
    public decimal PreviousQuantity { get; set; }
    
    // The amount added or subtracted (e.g., +10 or -5)
    public decimal QuantityChange { get; set; }
    
    // The new stock quantity after this adjustment
    public decimal NewQuantity { get; set; }
    
    // Corresponds to StockTransactionTypeId (Adjustment = 2, Damage = 3)
    public int StockTransactionTypeId { get; set; }

    // Navigation Properties
    public StockAdjustment? StockAdjustment { get; set; }
    public Product? Product { get; set; }
    public StockTransactionType? StockTransactionType { get; set; }
}
