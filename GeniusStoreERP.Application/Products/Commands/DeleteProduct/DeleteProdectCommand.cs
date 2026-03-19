using MediatR;

namespace GeniusStoreERP.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest;
