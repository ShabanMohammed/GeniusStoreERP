using MediatR;

namespace GeniusStoreERP.Application.Stock.Products.Commands.CreeteProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    decimal StockQuantity,
    decimal ReorderLevel,
    string? SKU,
    string? Barcode,
    int CategoryId
) : IRequest<int>;
