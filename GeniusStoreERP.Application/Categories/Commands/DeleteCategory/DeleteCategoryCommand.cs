using MediatR;

namespace GeniusStoreERP.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest;
