using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;

public class GetGeneralSettingsQueryHandler : IRequestHandler<GetGeneralSettingsQuery, GeneralSettingsDto?>
{
    private readonly IApplicationDbContext _context;

    public GetGeneralSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GeneralSettingsDto?> Handle(GetGeneralSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.GeneralSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings == null)
            return null;

        return new GeneralSettingsDto(
            settings.CompanyName,
            settings.TaxPercentage,
            settings.CurrencySymbol
        );
    }
}
