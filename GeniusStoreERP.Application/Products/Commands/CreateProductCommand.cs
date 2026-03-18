using MediatR;

namespace GeniusStoreERP.Application.Products.Commands;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    decimal ReorderLevel,
    string? SKU,
    string? Barcode,
    int CategoryId
) : IRequest<int>;
