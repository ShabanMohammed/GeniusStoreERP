using FluentValidation;

namespace GeniusStoreERP.Application.Users.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("اسم المستخدم مطلوب");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("كلمة السر مطلوبة");

    }
}