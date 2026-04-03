using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.Application.GeneralSettings.Commands;

public record GeneralSettingUpdateCommand(
    string CompanyName,
    string? LegalName,
    string? Address,
    string? Phone1,
    string? Phone2,
    string? Email,
    string? Website,
    string? TaxNumber,
    byte[]? Logo,
    decimal TaxPercentage,
    string CurrencySymbol
) : IRequest<GeneralSettingsDto>;