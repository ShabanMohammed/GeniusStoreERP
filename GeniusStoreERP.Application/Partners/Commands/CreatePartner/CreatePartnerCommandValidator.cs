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
            .EmailAddress().WithMessage("تنسيق البريد الإلكتروني غير صحيح.");

        RuleFor(x => x.PhoneNumber)
                        .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("تنسيق رقم الهاتف غير صحيح.");

        RuleFor(x => x.Address)
                      .MaximumLength(200).WithMessage("لا يمكن أن يتجاوز العنوان 200 حرف.");
    }
}
