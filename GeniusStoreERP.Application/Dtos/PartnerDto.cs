namespace GeniusStoreERP.Application.Dtos;

public record PartnerDto
(
    int Id,
    string Name,
    string Email,
    string PhoneNumber,
    string Address,
    bool IsSupplier,
    bool IsCustomer);