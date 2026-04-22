using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GeniusStoreERP.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            throw new NotFoundException();
        }

        user.FullName = request.FullName;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded && !string.IsNullOrWhiteSpace(request.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.Role);
        }

        return result.Succeeded;
    }
}
