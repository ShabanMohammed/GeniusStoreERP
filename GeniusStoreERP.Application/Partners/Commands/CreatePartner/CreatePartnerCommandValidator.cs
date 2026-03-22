using FluentValidation;

namespace GeniusStoreERP.Application.Partners.Commands.CreatePartner;

public class CreatePartnerCommandValidator : AbstractValidator<CreatePartnerCommand>
{
    public CreatePartnerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("الاسم مطلوب.")
            .MaximumLength(100).WithMessage("لا يمكن أن يتجاوز الاسم 100 حرف.");

        RuleFor(x => x.Email)
            .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("تنسيق البريد الإلكتروني غير صحيح.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[+\d\s\-\(\).]{7,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("تنسيق رقم الهاتف غير صحيح (يقبل الأرقام المحلية والدولية).");

        RuleFor(x => x.Address)
            .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Address))
                .WithMessage("لا يمكن أن يتجاوز العنوان 200 حرف.");
    }
}
