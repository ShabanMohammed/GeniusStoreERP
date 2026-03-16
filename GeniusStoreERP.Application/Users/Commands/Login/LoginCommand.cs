using MediatR;

namespace GeniusStoreERP.Application.Users.Commands.Login;

public record LoginCommand(string UserName, string Password) : IRequest<LoginResponse>;
