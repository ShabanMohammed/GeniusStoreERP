namespace GeniusStoreERP.Application.Dtos
{
    public record PartnerAccountDto(
        int Id,
        string Name,
        string PhoneNumber,
        bool IsSupplier,
        bool IsCustomer,
        decimal TotalDebit,
        decimal TotalCredit,
        decimal Balance
    );
}
