using FluentValidation;
using GeniusStoreERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Products.Commands.CreeteProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IApplicationDbContext dbContext;
    public CreateProductCommandValidator(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("اسم المنتج مطلوب.")
            .MaximumLength(100).WithMessage("اسم المنتج يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("سعر البيع لا يمكن أن يكون قيمة سالبة.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0).WithMessage("حد الطلب لا يمكن أن يكون قيمة سالبة.");

        RuleFor(x => x.SKU)
            .MaximumLength(50).WithMessage("كود الصنف (SKU) يجب ألا يتجاوز 50 حرفاً.");

        RuleFor(x => x.Barcode)
            .MaximumLength(50).WithMessage("الباركود يجب ألا يتجاوز 50 حرفاً.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("يجب اختيار قسم للمنتج.");
        RuleFor(x => x).MustAsync(async (x, cancellationToken) =>
        {
            return await dbContext.Categories.AnyAsync(c => c.Name == x.Name, cancellationToken);
        }).WithMessage("اسم الصنف يجب ان يكون فريد.");
    }
}