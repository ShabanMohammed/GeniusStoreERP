using GeniusStoreERP.Application.Dtos;

namespace GeniusStoreERP.Application.Common.Interfaces;

public interface IInvoiceReportService
{
    byte[] GeneratePdf(InvoiceDto invoice, GeneralSettingsDto? settings);
    byte[] GenerateExcel(InvoiceDto invoice, GeneralSettingsDto? settings);
    byte[] GenerateWord(InvoiceDto invoice, GeneralSettingsDto? settings);
}
