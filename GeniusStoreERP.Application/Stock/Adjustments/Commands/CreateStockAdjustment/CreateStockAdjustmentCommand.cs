using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities.Stock;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusStoreERP.Application.Stock.Adjustments.Commands.CreateStockAdjustment;

public record CreateStockAdjustmentCommand : IRequest<int>
{
    public DateTime AdjustmentDate { get; init; } = DateTime.Now;
    public string? Remarks { get; init; }
    public List<CreateStockAdjustmentItemCommand> Items { get; init; } = new();
}

public record CreateStockAdjustmentItemCommand
{
    public int ProductId { get; init; }
    public decimal QuantityChange { get; init; }
    public int StockTransactionTypeId { get; init; }
}

public class CreateStockAdjustmentCommandHandler : IRequestHandler<CreateStockAdjustmentCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateStockAdjustmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateStockAdjustmentCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate Reference Number
        var lastAdj = await _context.StockAdjustments
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        var nextId = (lastAdj?.Id ?? 0) + 1;
        var referenceNumber = $"ADJ-{DateTime.Now:yyyyMM}-{nextId:D4}";

        // 2. Create Header
        var adjustment = new StockAdjustment
        {
            AdjustmentDate = request.AdjustmentDate,
            ReferenceNumber = referenceNumber,
            Remarks = request.Remarks
        };

        await _context.StockAdjustments.AddAsync(adjustment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken); // Save to get the Id

        // 3. Process Items
        foreach (var itemCommand in request.Items)
        {
            var product = await _context.Products.FindAsync(new object[] { itemCommand.ProductId }, cancellationToken);
            if (product == null) continue;

            var oldQuantity = product.StockQuantity;
            var newQuantity = oldQuantity + itemCommand.QuantityChange;

            // Update Product Stock
            product.StockQuantity = newQuantity;

            // Create Adjustment Item
            var adjItem = new StockAdjustmentItem
            {
                StockAdjustmentId = adjustment.Id,
                ProductId = itemCommand.ProductId,
                PreviousQuantity = oldQuantity,
                QuantityChange = itemCommand.QuantityChange,
                NewQuantity = newQuantity,
                StockTransactionTypeId = itemCommand.StockTransactionTypeId
            };
            await _context.StockAdjustmentItems.AddAsync(adjItem, cancellationToken);

            // Create Stock Transaction for Audit
            var stockTransaction = new StockTransaction
            {
                ProductId = itemCommand.ProductId,
                Quantity = itemCommand.QuantityChange,
                StockTransactionTypeId = itemCommand.StockTransactionTypeId,
                TransactionDate = request.AdjustmentDate,
                AdjustmentId = adjustment.Id,
                AdjustmentReference = referenceNumber,
                Remarks = $"تسوية مخزنية: {request.Remarks}"
            };
            await _context.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return adjustment.Id;
    }
}
