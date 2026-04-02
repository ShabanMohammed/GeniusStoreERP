using FluentValidation;

namespace GeniusStoreERP.Application.Transactions.Queries.GetInvoiceByIdQuery;

public class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
{
    public GetInvoiceByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("معرف الفاتورة يجب أن يكون أكبر من صفر");
    }
}