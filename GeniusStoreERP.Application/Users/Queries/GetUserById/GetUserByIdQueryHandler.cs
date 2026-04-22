using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GeniusStoreERP.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            throw new NotFoundException();
        }

        var roles = await _userManager.GetRolesAsync(user);
        
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "مستخدم"
        };
    }
}
