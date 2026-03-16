using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GeniusStoreERP.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        return result.Succeeded;
    }
}
