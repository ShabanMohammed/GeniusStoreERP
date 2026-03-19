using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;
