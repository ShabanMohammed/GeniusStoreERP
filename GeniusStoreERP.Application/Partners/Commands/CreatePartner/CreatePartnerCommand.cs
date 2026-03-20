using MediatR;

namespace GeniusStoreERP.Application.Partners.Commands.CreatePartner;

public record CreatePartnerCommand
(
    string Name,
    string Email,
    string PhoneNumber,
    string Address,
    bool IsSupplier,
    bool IsCustomer)
: IRequest<int>;
