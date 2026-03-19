using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities;
using MediatR;

namespace GeniusStoreERP.Application.Products.Commands.CreeteProduct;

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
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ReorderLevel = request.ReorderLevel,
            SKU = request.SKU,
            Barcode = request.Barcode,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow

        };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}