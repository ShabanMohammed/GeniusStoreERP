using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;
using System.Collections.Generic;

namespace GeniusStoreERP.UI.Services;

public class PartnerReportService : IPartnerReportService
{
    public byte[] GeneratePartnerSummaryPdf(List<PartnerAccountDto> accounts, GeneralSettingsDto? settings)
    {
        var document = new PartnerSummaryReportDocument(accounts, settings);
        return document.GeneratePdf();
    }

    public byte[] GeneratePartnerStatementPdf(PartnerStatementDto statement, GeneralSettingsDto? settings)
    {
        var document = new PartnerStatementReportDocument(statement, settings);
        return document.GeneratePdf();
    }

    public byte[] GenerateDebtAgingPdf(List<DebtAgingDto> agingData, GeneralSettingsDto? settings)
    {
        var document = new DebtAgingReportDocument(agingData, settings);
        return document.GeneratePdf();
    }
}
