using GeniusStoreERP.Application.Dtos;

namespace GeniusStoreERP.Application.Common.Interfaces;

public interface IFinanceReportService
{
    byte[] GenerateTreasuryReceiptPdf(TreasuryTransactionDto transaction, GeneralSettingsDto? settings);
}
