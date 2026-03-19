using MediatR;

namespace GeniusStoreERP.Application.Stock.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest;
