using System.Collections.Generic;
using GeniusStoreERP.Application.Dtos;

namespace GeniusStoreERP.Application.Common.Interfaces
{
    public interface IPartnerReportService
    {
        byte[] GeneratePartnerSummaryPdf(List<PartnerAccountDto> accounts, GeneralSettingsDto? settings);
        byte[] GeneratePartnerStatementPdf(PartnerStatementDto statement, GeneralSettingsDto? settings);
    }
}
