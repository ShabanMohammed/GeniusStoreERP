using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IApplicationDbContext dbContext;

    public DeleteCategoryCommandHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FindAsync(request.Id);

        if (category == null)
        {
            throw new NotFoundException(request.Id);
        }
        var rsulte = await dbContext.Products.CountAsync(p => p.CategoryId == request.Id, cancellationToken);
        if (rsulte > 0)
        {
            throw new BusinessException("لا يمكن حذف هذا التصنيف لأنه مرتبط بمنتجات");
        }

        category.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}