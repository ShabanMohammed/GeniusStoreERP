using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace GeniusStoreERP.UI.Services;

public class LowStockReportDocument : IDocument
{
    private readonly List<ProductDto> _products;
    private readonly GeneralSettingsDto? _settings;

    public LowStockReportDocument(List<ProductDto> products, GeneralSettingsDto? settings)
    {
        _products = products;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Calibri"));
            page.ContentFromRightToLeft();

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(infoColumn =>
                {
                    var companyName = _settings?.CompanyName ?? "Genius Store ERP";
                    infoColumn.Item().Text(companyName).FontSize(18).SemiBold().FontColor("#1E3A8A");
                    infoColumn.Item().Text("تقرير نواقص الأصناف").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);
                });

                row.RelativeItem().AlignLeft().Column(reportColumn =>
                {
                    reportColumn.Item().Text($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}").FontSize(10);
                    reportColumn.Item().Text($"وقت التقرير: {DateTime.Now:HH:mm}").FontSize(10);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor("#E5E7EB");
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);   // #
                    columns.RelativeColumn(3);    // Product Name
                    columns.RelativeColumn(1.5f); // Category
                    columns.RelativeColumn(1f);   // Stock
                    columns.RelativeColumn(1f);   // Reorder Level
                    columns.RelativeColumn(1f);   // Diff
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("م");
                    header.Cell().Element(HeaderStyle).Text("الصنف");
                    header.Cell().Element(HeaderStyle).Text("التصنيف");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الرصيد");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("حد الطلب");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("العجز");

                    static IContainer HeaderStyle(IContainer container) => container.Background("#1E3A8A").Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(10));
                });

                int index = 1;
                foreach (var product in _products)
                {
                    table.Cell().Element(CellStyle).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(CellStyle).Text(product.Name);
                    table.Cell().Element(CellStyle).Text(product.CategoryName);
                    table.Cell().Element(CellStyle).AlignCenter().Text(product.StockQuantity?.ToString("N2") ?? "0");
                    table.Cell().Element(CellStyle).AlignCenter().Text(product.ReorderLevel?.ToString("N2") ?? "0");
                    
                    var diff = (product.ReorderLevel ?? 0) - (product.StockQuantity ?? 0);
                    table.Cell().Element(CellStyle).AlignCenter().Text(diff.ToString("N2")).FontColor(Colors.Red.Medium).SemiBold();

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).DefaultTextStyle(x => x.FontSize(10));
                }
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(20).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            column.Item().AlignCenter().Text(text =>
            {
                text.Span("نظام العبقري لإدارة المستودعات - Genius Store ERP - ").FontSize(9).FontColor(Colors.Grey.Medium);
                text.CurrentPageNumber().FontSize(9);
            });
        });
    }
}
