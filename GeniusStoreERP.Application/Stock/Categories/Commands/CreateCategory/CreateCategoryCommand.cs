using MediatR;

namespace GeniusStoreERP.Application.Stock.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description = null) : IRequest<int>;
