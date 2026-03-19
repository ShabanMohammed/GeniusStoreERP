using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities.Stock;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Products.Commands.CreeteProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IApplicationDbContext dbContext;
    public CreateProductCommandHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {

        var product = new Product
        {
            Name = request.Name.Sanitize()??string.Empty,
            Description = request.Description?.Sanitize(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ReorderLevel = request.ReorderLevel,
            SKU = request.SKU?.Sanitize(),
            Barcode = request.Barcode?.Sanitize(),
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow

        };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}