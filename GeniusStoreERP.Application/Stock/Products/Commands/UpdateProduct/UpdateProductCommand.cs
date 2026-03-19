using MediatR;

namespace GeniusStoreERP.Application.Stock.Products.Commands.UpdateProduct;

public record UpdateProductCommand(int Id,
string Name,
string? Description,
decimal Price,
decimal StockQuantity,
decimal ReorderLevel,
string? SKU,
string? Barcode,
int CategoryId
) : IRequest;
