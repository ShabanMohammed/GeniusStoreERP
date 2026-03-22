using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities.Transactions;

public class InvoiceType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;


    //navigation properties
    public ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
}

