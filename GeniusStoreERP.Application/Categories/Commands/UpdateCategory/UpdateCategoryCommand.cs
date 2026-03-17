using MediatR;

namespace GeniusStoreERP.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int Id, string Name, string? Description = null) : IRequest;
