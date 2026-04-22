using MediatR;

namespace GeniusStoreERP.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(string Id) : IRequest<bool>;
