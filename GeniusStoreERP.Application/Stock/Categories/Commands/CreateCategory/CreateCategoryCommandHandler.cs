using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities.Stock;
using MediatR;

namespace GeniusStoreERP.Application.Stock.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly IApplicationDbContext dbContext;

    public CreateCategoryCommandHandler(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name.Sanitize() ?? string.Empty,
            Description = request.Description?.Sanitize(),
            CreatedAt = DateTime.UtcNow
        };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return category.Id;
    }
}