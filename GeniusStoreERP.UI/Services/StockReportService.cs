using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using QuestPDF.Fluent;
using System.Collections.Generic;

namespace GeniusStoreERP.UI.Services;

public class StockReportService : IStockReportService
{
    public byte[] GenerateLowStockPdf(List<ProductDto> products, GeneralSettingsDto? settings)
    {
        var document = new LowStockReportDocument(products, settings);
        return document.GeneratePdf();
    }

    public byte[] GenerateInventoryValuePdf(List<ProductDto> products, GeneralSettingsDto? settings)
    {
        var document = new InventoryValueReportDocument(products, settings);
        return document.GeneratePdf();
    }

    public byte[] GenerateProductMovementPdf(ProductDto product, List<ProductTransactionDto> transactions, DateTime? startDate, DateTime? endDate, GeneralSettingsDto? settings)
    {
        var document = new ProductMovementReportDocument(product, transactions, startDate, endDate, settings);
        return document.GeneratePdf();
    }
}
