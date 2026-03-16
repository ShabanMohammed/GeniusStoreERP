using MediatR;

namespace GeniusStoreERP.Application.Users.Commands.CreateUser;

public record CreateUserCommand(string UserName, string Password, string FullName) : IRequest<bool>;

