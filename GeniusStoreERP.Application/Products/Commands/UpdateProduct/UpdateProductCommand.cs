using MediatR;

namespace GeniusStoreERP.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(int Id,
string Name,
string? Description,
decimal Price,
decimal ReorderLevel,
string? SKU,
string? Barcode,
int CategoryId
) : IRequest;
