using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    string? SearchText = null,
    int PageSize = 10,
    int PageNumber = 1
) : IRequest<PagedResponse<UserDto>>;
