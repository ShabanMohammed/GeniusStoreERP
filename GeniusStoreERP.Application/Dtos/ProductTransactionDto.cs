using System;

namespace GeniusStoreERP.Application.Dtos;

public class ProductTransactionDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public int StockTransactionTypeId { get; set; }
    public string? StockTransactionTypeName { get; set; }
    public DateTime TransactionDate { get; set; }
    public int? InvoiceId { get; set; }
    public string? InvoiceReference { get; set; }
    public int? AdjustmentId { get; set; }
    public string? AdjustmentReference { get; set; }
    public string? Remarks { get; set; }
}