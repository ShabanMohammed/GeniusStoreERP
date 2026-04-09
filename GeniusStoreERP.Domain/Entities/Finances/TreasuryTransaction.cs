using GeniusStoreERP.Domain.Common;
using GeniusStoreERP.Domain.Entities.Partners;
using GeniusStoreERP.Domain.Entities.Transactions;

namespace GeniusStoreERP.Domain.Entities.Finances;

public class TreasuryTransaction : BaseEntity
{
    public int TreasuryId { get; set; }
    public Treasury? Treasury { get; set; }
    
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public TreasuryTransactionType Type { get; set; }
    
    public int? PartnerId { get; set; }
    public Partner? Partner { get; set; }
    
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
  
}
