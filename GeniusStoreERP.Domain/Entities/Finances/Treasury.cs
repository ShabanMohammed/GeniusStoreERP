using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities.Finances;

public class Treasury : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string? Description { get; set; }
    
    public ICollection<TreasuryTransaction>? Transactions { get; set; }
}
