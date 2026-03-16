using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? SKU { get; set; } // Stock Keeping Unit
    public string? Barcode { get; set; }

    // Foreign Key
    public int CategoryId { get; set; }
    
    // Navigation Property
    public virtual Category Category { get; set; } = null!;
}
