using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(string Id) : IRequest<UserDto>;
