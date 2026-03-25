using GeniusStoreERP.Domain.Entities;

namespace GeniusStoreERP.Application.Dtos;

public record GeneralSettingsDto(
    string CompanyName,
    decimal TaxPercentage,
    string CurrencySymbol
);
