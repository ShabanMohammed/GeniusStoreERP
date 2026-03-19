using MediatR;

namespace GeniusStoreERP.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
