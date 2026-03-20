using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteProductCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = _dbContext.Products.FirstOrDefault(p => p.Id == request.Id);
        if (entity == null)
        {
            throw new NotFoundException();
        }

        entity.IsDeleted = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
