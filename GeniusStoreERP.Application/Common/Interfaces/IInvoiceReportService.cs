using GeniusStoreERP.Application.Dtos;

namespace GeniusStoreERP.Application.Common.Interfaces;

public interface IInvoiceReportService
{
    byte[] GeneratePdf(InvoiceDto invoice);
    byte[] GenerateExcel(InvoiceDto invoice);
    byte[] GenerateWord(InvoiceDto invoice);
}
