using FluentValidation;
using GeniusStoreERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IApplicationDbContext dbContext;


    public UpdateCategoryCommandValidator(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم التصنيف مطلوب")
            .MaximumLength(100).WithMessage("اسم التصنيف لا يمكن أن يتجاوز 100 حرف");
        RuleFor(x => x).MustAsync(async (x, cancellation) =>
        {

            return !await dbContext.Categories.AnyAsync(c => c.Name == x.Name && c.Id != x.Id, cancellation);

        }).WithMessage("اسم التصنيف يجب أن يكون فريدًا");
    }
}
