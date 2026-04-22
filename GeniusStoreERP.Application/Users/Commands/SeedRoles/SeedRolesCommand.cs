using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GeniusStoreERP.Application.Users.Commands.SeedRoles;

public record SeedRolesCommand : IRequest<bool>;

public class SeedRolesCommandHandler : IRequestHandler<SeedRolesCommand, bool>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedRolesCommandHandler(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<bool> Handle(SeedRolesCommand request, CancellationToken cancellationToken)
    {
        string[] roles = { "Admin", "Sales", "Warehouse", "Accountant" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        return true;
    }
}
