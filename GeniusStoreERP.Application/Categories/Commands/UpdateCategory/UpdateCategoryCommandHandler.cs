using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IApplicationDbContext dbContext;
    public UpdateCategoryCommandHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException("التصنيف غير موجود");
        }
        category.Name = request.Name;
        category.Description = request.Description;
        var result = await dbContext.SaveChangesAsync(cancellationToken);

    }
}