using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery(
    string? categoryName = null,
    int PageNumber = 1,
    int PageSize = 10
    ) : IRequest<PagedResponse<CategoryDto>>;
