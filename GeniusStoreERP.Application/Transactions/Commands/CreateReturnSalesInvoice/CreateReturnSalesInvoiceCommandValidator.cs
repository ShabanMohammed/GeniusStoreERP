using FluentValidation;

namespace GeniusStoreERP.Application.Transactions.Commands.CreateReturnSalesInvoice;

public class CreateReturnSalesInvoiceCommandValidator : AbstractValidator<CreateReturnSalesInvoiceCommand>
{
    public CreateReturnSalesInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceDate).NotEmpty().WithMessage("التاريخ مطلوب");
        RuleFor(x => x.PartnerId).NotEmpty().WithMessage("العميل مطلوب");
        RuleFor(x => x.InvoiceItems).NotEmpty().WithMessage("يجب إضافة صنف واحد على الأقل بالفاتورة");

        RuleForEach(x => x.InvoiceItems).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId).NotEmpty().WithMessage("يجب اختيار منتج صحيح");
            items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("الكمية يجب أن تكون أكبر من الصفر");
            items.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("سعر الوحدة لا يمكن أن يكون سالباً");
        });
    }
}
