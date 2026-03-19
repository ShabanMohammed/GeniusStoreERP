using System.Collections.Generic;
using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities.Stock;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Navigation Property
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
