using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GeniusStoreERP.Application.GeneralSettings.Commands;

public class GeneralSettingUpdateCommandHandler : IRequestHandler<GeneralSettingUpdateCommand, GeneralSettingsDto>
{
    private readonly IApplicationDbContext _context;

    public GeneralSettingUpdateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GeneralSettingsDto> Handle(GeneralSettingUpdateCommand request, CancellationToken cancellationToken)
    {
        var settings = await _context.GeneralSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings == null)
        {
            settings = new GeneralSetting();
            _context.GeneralSettings.Add(settings);
        }

        settings.CompanyName = request.CompanyName;
        settings.LegalName = request.LegalName;
        settings.Address = request.Address;
        settings.Phone1 = request.Phone1;
        settings.Phone2 = request.Phone2;
        settings.Email = request.Email;
        settings.Website = request.Website;
        settings.TaxNumber = request.TaxNumber;
        settings.Logo = request.Logo;
        settings.TaxPercentage = request.TaxPercentage;
        settings.CurrencySymbol = request.CurrencySymbol;

        await _context.SaveChangesAsync(cancellationToken);

return new GeneralSettingsDto(
    settings.CompanyName,
    settings.LegalName,
    settings.Address,
    settings.Phone1,
    settings.Phone2,
    settings.Email,
    settings.Website,
    settings.TaxNumber,
    settings.Logo,
    settings.TaxPercentage,
    settings.CurrencySymbol
);
    }
}