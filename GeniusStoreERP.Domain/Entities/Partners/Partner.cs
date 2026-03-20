using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities.Partners;

public class Partner : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsCustomer { get; set; }
    public bool IsSupplier { get; set; }
}