using System;
using System.Collections.Generic;
using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

    // Foreign Key
    public int CustomerId { get; set; }
    
    // Navigation Property
    public virtual Customer Customer { get; set; } = null!;
}
