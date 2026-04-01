using GeniusStoreERP.Domain.Common;
using GeniusStoreERP.Domain.Entities.Partners;
using GeniusStoreERP.Domain.Entities.Transactions;

namespace GeniusStoreERP.Domain.Entities.Finances;

public class PartnerTransaction : BaseEntity
{
    public int PartnerId { get; set; }
    public Partner? Partner { get; set; }
    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public int? PaymentId { get; set; }
    //public Payment? Payment { get; set; }
    public DateTime TransactionDate { get; set; }
    public int TransactionTypeId { get; set; }
    public PartnerTransactionType? Type { get; set; }

    public decimal Debit { get; set; }  // مدين (قيمة تزيد مديونية العميل أو تنقص مديونية المورد)
    public decimal Credit { get; set; } // دائن (قيمة تنقص مديونية العميل أو تزيد مديونية المورد)

    public string? ReferenceNumber { get; set; }
    public string? Remarks { get; set; }
}
