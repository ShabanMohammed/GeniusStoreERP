using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GeniusStoreERP.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            throw new NotFoundException();
        }

        // Optional: Implement soft delete if preferred
        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded;
    }
}
