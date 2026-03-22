using GeniusStoreERP.Application.Dtos.ListItemDto;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Products.Queries.GetProductItems;

public record GetProductItemsCommand() : IRequest<List<ProductListItemDto>>;
