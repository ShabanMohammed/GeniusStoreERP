using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GeniusStoreERP.Application.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public LoginCommandHandler(UserManager<ApplicationUser> userManager)
    {
            _userManager = userManager;
    }
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null || !await _userManager.CheckPasswordAsync(user ,request.Password))
        {
           throw new UnauthorizedAccessException("يوجد خطأ في اسم المستخدم أو كلمة المرور");
        }
       return new LoginResponse(user.Id, user.UserName!, user.FullName);

    }
}

