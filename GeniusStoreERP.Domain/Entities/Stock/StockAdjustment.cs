using GeniusStoreERP.Domain.Common;
using System.Collections.Generic;

namespace GeniusStoreERP.Domain.Entities.Stock;

public class StockAdjustment : BaseEntity
{
    public DateTime AdjustmentDate { get; set; } = DateTime.Now;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string? Remarks { get; set; }

    // Navigation Property
    public ICollection<StockAdjustmentItem> Items { get; set; } = new List<StockAdjustmentItem>();
    public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
}
