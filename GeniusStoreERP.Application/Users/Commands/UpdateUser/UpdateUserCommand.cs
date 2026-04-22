using MediatR;

namespace GeniusStoreERP.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    string Id,
    string FullName,
    string Email,
    string Role
) : IRequest<bool>;
