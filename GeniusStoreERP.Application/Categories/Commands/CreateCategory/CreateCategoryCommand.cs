using FluentValidation;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description = null) : IRequest<int>;


public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>

{
    private readonly IApplicationDbContext dbContext;

    public CreateCategoryCommandValidator(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم التصنيف مطلوب")
            .MaximumLength(100).WithMessage("اسم التصنيف لا يمكن أن يتجاوز 100 حرف");
        RuleFor(x => x).MustAsync(async (x, cancellation) =>
        {

            return !await dbContext.Categories.AnyAsync(c => c.Name == x.Name, cancellation);

        }).WithMessage("اسم التصنيف يجب أن يكون فريدًا");

    }
}

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
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        return category.Id;
    }
}