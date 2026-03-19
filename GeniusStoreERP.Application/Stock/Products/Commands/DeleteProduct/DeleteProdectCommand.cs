using MediatR;

namespace GeniusStoreERP.Application.Stock.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest;
