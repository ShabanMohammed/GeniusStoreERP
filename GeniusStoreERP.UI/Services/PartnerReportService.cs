using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using System.Collections.Generic;

namespace GeniusStoreERP.UI.Services;

public class PartnerReportService : IPartnerReportService
{
    public byte[] GeneratePartnerSummaryPdf(List<PartnerAccountDto> accounts, GeneralSettingsDto? settings)
    {
        // Placeholder implementation
        return System.Array.Empty<byte>();
    }

    public byte[] GeneratePartnerStatementPdf(PartnerStatementDto statement, GeneralSettingsDto? settings)
    {
        // Placeholder implementation
        return System.Array.Empty<byte>();
    }
}
