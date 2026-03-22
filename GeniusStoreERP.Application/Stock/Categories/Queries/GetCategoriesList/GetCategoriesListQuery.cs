using GeniusStoreERP.Application.Dtos.ListItemDto;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Categories.Queries.GetCategoriesList;

public record GetCategoriesListQuery : IRequest<List<CategoryListItemDto>>;
