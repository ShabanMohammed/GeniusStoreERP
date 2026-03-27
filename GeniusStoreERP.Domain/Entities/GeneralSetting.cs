using GeniusStoreERP.Domain.Common;

namespace GeniusStoreERP.Domain.Entities;

public class GeneralSetting : BaseEntity
{
    public string CompanyName { get; set; } = null!;
    public string? LegalName { get; set; }
    public string? Address { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? TaxNumber { get; set; }
    public byte[]? Logo { get; set; }
    public decimal TaxPercentage { get; set; } = 14;
    public string CurrencySymbol { get; set; } = "EGP";
}