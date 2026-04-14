using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace GeniusStoreERP.UI.Services;

public class InventoryValueReportDocument : IDocument
{
    private readonly List<ProductDto> _products;
    private readonly GeneralSettingsDto? _settings;

    public InventoryValueReportDocument(List<ProductDto> products, GeneralSettingsDto? settings)
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
                if (_settings?.Logo != null && _settings.Logo.Length > 0)
                {
                    row.ConstantItem(80).AlignRight().Column(logoColumn =>
                    {
                        logoColumn.Item().Width(70).Height(70).Image(_settings.Logo);
                    });
                }

                row.RelativeItem().PaddingRight(10).Column(infoColumn =>
                {
                    var companyName = _settings?.CompanyName ?? "Genius Store ERP";
                    infoColumn.Item().Text(companyName).FontSize(18).SemiBold().FontColor("#1E3A8A");
                    infoColumn.Item().Text("تقرير جرد المخزون وتقدير القيمة").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);
                });

                row.RelativeItem().AlignLeft().Column(reportColumn =>
                {
                    reportColumn.Item().Text($"تاريخ الجرد: {DateTime.Now:yyyy/MM/dd}").FontSize(10);
                    reportColumn.Item().Text($"إجمالي الأصناف: {_products.Count}").FontSize(10);
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
                    columns.RelativeColumn(1f);   // Current Stock
                    columns.RelativeColumn(1.2f); // Cost Price
                    columns.RelativeColumn(1.5f); // Total Value
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("م");
                    header.Cell().Element(HeaderStyle).Text("الصنف");
                    header.Cell().Element(HeaderStyle).Text("التصنيف");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الكمية");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("سعر التكلفة");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("إجمالي القيمة");

                    static IContainer HeaderStyle(IContainer container) => container.Background("#1E3A8A").Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(10));
                });

                decimal totalInventoryValue = 0;
                int index = 1;
                foreach (var product in _products)
                {
                    var stock = product.StockQuantity ?? 0;
                    var cost = product.Price; // Assuming Price here is cost or we should have a cost field
                    var lineTotal = stock * cost;
                    totalInventoryValue += lineTotal;

                    table.Cell().Element(CellStyle).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(CellStyle).Text(product.Name);
                    table.Cell().Element(CellStyle).Text(product.CategoryName);
                    table.Cell().Element(CellStyle).AlignCenter().Text(stock.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(cost.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(lineTotal.ToString("N2"));

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).DefaultTextStyle(x => x.FontSize(10));
                }

                // Total Summary Row
                table.Footer(footer =>
                {
                    footer.Cell().ColumnSpan(5).Padding(6).AlignLeft().Text("إجمالي قيمة المخزون:").Bold();
                    footer.Cell().Padding(6).AlignCenter().Text(totalInventoryValue.ToString("N2")).Bold().FontColor("#1E3A8A");
                });
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
