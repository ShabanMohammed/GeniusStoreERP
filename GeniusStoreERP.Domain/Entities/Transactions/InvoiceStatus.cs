namespace GeniusStoreERP.Domain.Entities.Transactions;

public class InvoiceStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    //navigation properties
    public ICollection<Invoice> Invoices { get; set; } = default!;
}