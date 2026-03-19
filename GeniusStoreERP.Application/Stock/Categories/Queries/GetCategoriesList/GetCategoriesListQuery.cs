using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Categories.Queries.GetCategoriesList;

public record GetCategoriesListQuery : IRequest<List<CategoryListItemDto>>;
