using FluentValidation;
using GeniusStoreERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Stock.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IApplicationDbContext dbContext;

    public UpdateProductCommandValidator(IApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        RuleFor(x => x.Name).NotEmpty().WithMessage("اسم المنتج مطلوب.")
        .MaximumLength(100).WithMessage("اسم المنتج لا يمكن أن يتجاوز 100 حرف.");
        RuleFor(x => x.Description).MaximumLength(500).WithMessage("وصف المنتج لا يمكن أن يتجاوز 500 حرف.");
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("سعر المنتج يجب أن يكون رقمًا موجبًا.");
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0).WithMessage(" مستوى إعادة الطلب يجب أن يكون رقمًا موجبًا.");
        RuleFor(x => x.SKU).MaximumLength(50).WithMessage("رمز المنتج لا يمكن أن يتجاوز 50 حرف.");
        RuleFor(x => x.Barcode).MaximumLength(50).WithMessage("الباركود لا يمكن أن يتجاوز 50 حرف.");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("معرف الفئة يجب أن يكون رقمًا موجبًا.");
        RuleFor(x => x).MustAsync(async (x, cancellationToken) =>

        {
            return !await dbContext.Categories.AnyAsync(c => c.Name == x.Name && c.Id != x.CategoryId, cancellationToken);
        }).WithMessage("اسم الصنف يجب ان يكون فريد.");

    }
}
