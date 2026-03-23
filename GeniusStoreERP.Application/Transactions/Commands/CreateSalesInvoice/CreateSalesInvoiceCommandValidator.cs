using FluentValidation;

namespace GeniusStoreERP.Application.Transactions.Commands.CreateSalesInvoice;

public class CreateSalesInvoiceCommandValidator : AbstractValidator<CreateSalesInvoiceCommand>
{
    public CreateSalesInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceDate).NotEmpty().WithMessage("التاريخ مطلوب");
        RuleFor(x => x.PartnerId).NotEmpty().WithMessage("العميل مطلوب");

    }
}