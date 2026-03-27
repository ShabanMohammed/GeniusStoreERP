namespace GeniusStoreERP.Domain.Entities.Stock;

public class StockTransactionType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<StockTransaction>? StockTransactions { get; set; } = new List<StockTransaction>();
}