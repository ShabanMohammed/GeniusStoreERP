using FluentValidation;
using GeniusStoreERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Categories.Commands.CreateCategory;

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
