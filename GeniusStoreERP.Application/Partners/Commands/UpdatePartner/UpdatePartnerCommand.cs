using MediatR;

namespace GeniusStoreERP.Application.Partners.Commands.UpdatePartner;

public record UpdatePartnerCommand
(
     int Id,
     string Name,
     string Email,
     string PhoneNumber,
     string Address,
     bool IsCustomer,
     bool IsSupplier
)
 : IRequest;
