using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;

namespace GeniusStoreERP.UI.Services;

public class FinanceReportService : IFinanceReportService
{
    public byte[] GenerateTreasuryReceiptPdf(TreasuryTransactionDto transaction, GeneralSettingsDto? settings)
    {
        var document = new TreasuryReceiptDocument(transaction, settings);
        return document.GeneratePdf();
    }
}
