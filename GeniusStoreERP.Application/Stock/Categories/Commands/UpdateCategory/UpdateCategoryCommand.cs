using MediatR;

namespace GeniusStoreERP.Application.Stock.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int Id, string Name, string? Description = null) : IRequest;
