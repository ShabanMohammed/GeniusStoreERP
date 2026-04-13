using System.Collections.Generic;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;

namespace GeniusStoreERP.Application.Common.Interfaces;

public interface IStockReportService
{
    byte[] GenerateLowStockPdf(List<ProductDto> products, GeneralSettingsDto? settings);
    byte[] GenerateInventoryValuePdf(List<ProductDto> products, GeneralSettingsDto? settings);
}
