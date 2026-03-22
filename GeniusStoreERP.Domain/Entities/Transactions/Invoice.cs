using System;
using GeniusStoreERP.Domain.Common;
using GeniusStoreERP.Domain.Entities.Partners;

namespace GeniusStoreERP.Domain.Entities.Transactions;

public class Invoice : BaseEntity
{
    public int InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalItemsAmount { get; set; }
    public decimal TotalItemsDiscount { get; set; }
    public decimal TotalItemsTax { get; set; }
    public decimal FinalAmount { get; set; }

    public string? Notes { get; set; }

    //navigation properties
    public int PartnerId { get; set; }
    public Partner? Partner { get; set; }

    public int InvoiceStatusId { get; set; }
    public InvoiceStatus? InvoiceStatus { get; set; }

    public int InvoiceTypeId { get; set; }
    public InvoiceType? InvoiceType { get; set; }


    public ICollection<InvoiceItem>? InvoiceItems { get; set; }

}
