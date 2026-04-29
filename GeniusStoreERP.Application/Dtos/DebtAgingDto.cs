namespace GeniusStoreERP.Application.Dtos
{
    public record DebtAgingDto(
        int PartnerId,
        string PartnerName,
        string? PhoneNumber,
        decimal TotalBalance,
        decimal Current,      // 0-30 days
        decimal ThirtyToSixty, // 31-60 days
        decimal SixtyToNinety, // 61-90 days
        decimal OverNinety    // 90+ days
    );
}
