using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Stock.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IApplicationDbContext dbContext;

    public UpdateProductCommandHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);
        if (product == null)
        {
            throw new NotFoundException("المنتج غير موجود.");
        }

        product.Name = request.Name.Sanitize() ?? string.Empty;
        product.Description = request.Description?.Sanitize();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.ReorderLevel = request.ReorderLevel;
        product.SKU = request.SKU?.Sanitize();
        product.Barcode = request.Barcode?.Sanitize();
        product.CategoryId = request.CategoryId;

        await dbContext.SaveChangesAsync(cancellationToken);

    }
}