using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    
    // Navigation Property
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
