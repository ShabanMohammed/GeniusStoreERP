using GeniusStoreERP.Application.Products.Queries.GetProductById;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
