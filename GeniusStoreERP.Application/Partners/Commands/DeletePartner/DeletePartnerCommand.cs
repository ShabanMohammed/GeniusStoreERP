using MediatR;

namespace GeniusStoreERP.Application.Partners.Commands.DeletePartner;

public record DeletePartnerCommand(int Id) : IRequest;
