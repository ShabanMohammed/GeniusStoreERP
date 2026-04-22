using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResponse<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PagedResponse<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            query = query.Where(u => u.FullName.Contains(request.SearchText) || 
                                    u.UserName!.Contains(request.SearchText) || 
                                    u.Email!.Contains(request.SearchText));
        }

        var count = await query.CountAsync(cancellationToken);
        
        var users = await query
            .OrderBy(u => u.UserName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "مستخدم"
            });
        }

        return new PagedResponse<UserDto>(userDtos, count, request.PageNumber, request.PageSize);
    }
}
