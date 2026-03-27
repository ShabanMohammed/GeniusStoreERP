using GeniusStoreERP.Domain.Common;
using GeniusStoreERP.Domain.Entities.Partners;
using GeniusStoreERP.Domain.Entities.Transactions;

namespace GeniusStoreERP.Domain.Entities.Finances;

public class PartnerTransaction : BaseEntity
{
    public int PartnerId { get; set; }
    public Partner Partner { get; set; }

    public int? InvoiceId { get; set; } // ربط بحركة فاتورة
    public Invoice? Invoice { get; set; }

    public int? PaymentId { get; set; } // ربط بسند قبض أو صرف
    //public Payment? Payment { get; set; }

    public DateTime TransactionDate { get; set; }
    private int TransactionTypeId { get; set; }
    public PartnerTransactionType? Type { get; set; } // (فاتورة، سند، مرتجع، رصيد أول المدة)

    // القيم المالية
    public decimal Debit { get; set; }  // مدين (قيمة تزيد مديونية العميل أو تنقص مديونية المورد)
    public decimal Credit { get; set; } // دائن (قيمة تنقص مديونية العميل أو تزيد مديونية المورد)

    public string? ReferenceNumber { get; set; } // رقم الفاتورة أو السند
    public string? Remarks { get; set; } // شرح الحركة
}
