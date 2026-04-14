using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;

namespace GeniusStoreERP.UI.Services;

public class ProductMovementReportDocument : IDocument
{
    private readonly ProductDto _product;
    private readonly List<ProductTransactionDto> _transactions;
    private readonly DateTime? _startDate;
    private readonly DateTime? _endDate;
    private readonly GeneralSettingsDto? _settings;

    public ProductMovementReportDocument(
        ProductDto product, 
        List<ProductTransactionDto> transactions, 
        DateTime? startDate, 
        DateTime? endDate, 
        GeneralSettingsDto? settings)
    {
        _product = product;
        _transactions = transactions;
        _startDate = startDate;
        _endDate = endDate;
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
                    infoColumn.Item().Text("كشف حركة صنف تفصيلي").FontSize(14).Bold().FontColor(Colors.Grey.Darken2);
                });

                row.RelativeItem().AlignLeft().Column(reportColumn =>
                {
                    reportColumn.Item().Text($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}").FontSize(10);
                    if (_startDate.HasValue || _endDate.HasValue)
                    {
                        var period = $"الفترة: ";
                        if (_startDate.HasValue) period += $"من {_startDate.Value:yyyy/MM/dd} ";
                        if (_endDate.HasValue) period += $"إلى {_endDate.Value:yyyy/MM/dd}";
                        reportColumn.Item().Text(period).FontSize(10);
                    }
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor("#E5E7EB");

            // Product Information Summary
            column.Item().Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(t => { t.Span("الصنف: ").Bold(); t.Span(_product.Name); });
                    c.Item().Text(t => { t.Span("الكود (SKU): ").Bold(); t.Span(_product.SKU ?? "N/A"); });
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(t => { t.Span("التصنيف: ").Bold(); t.Span(_product.CategoryName ?? "N/A"); });
                    c.Item().Text(t => { t.Span("الرصيد الحالي: ").Bold(); t.Span(_product.StockQuantity?.ToString("N2") ?? "0.00"); });
                });
            });
            
            column.Item().PaddingVertical(10);
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
                    columns.RelativeColumn(1.5f); // Date
                    columns.RelativeColumn(1.2f); // Type
                    columns.RelativeColumn(1f);   // Quantity
                    columns.RelativeColumn(1.5f); // Reference
                    columns.RelativeColumn(2f);   // Remarks
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("م");
                    header.Cell().Element(HeaderStyle).Text("التاريخ");
                    header.Cell().Element(HeaderStyle).Text("النوع");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الكمية");
                    header.Cell().Element(HeaderStyle).Text("المرجع");
                    header.Cell().Element(HeaderStyle).Text("ملاحظات");

                    static IContainer HeaderStyle(IContainer container) => container.Background("#1E3A8A").Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(10));
                });

                int index = 1;
                foreach (var trans in _transactions)
                {
                    table.Cell().Element(CellStyle).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(CellStyle).Text(trans.TransactionDate.ToString("yyyy/MM/dd HH:mm"));
                    table.Cell().Element(CellStyle).Text(trans.StockTransactionTypeName);
                    table.Cell().Element(CellStyle).AlignCenter().Text(trans.Quantity.ToString("N2"));
                    
                    var reference = !string.IsNullOrEmpty(trans.InvoiceReference) ? trans.InvoiceReference : trans.AdjustmentReference;
                    table.Cell().Element(CellStyle).Text(reference ?? "-");
                    
                    table.Cell().Element(CellStyle).Text(trans.Remarks ?? "");

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).DefaultTextStyle(x => x.FontSize(9));
                }
            });

            if (!_transactions.Any())
            {
                column.Item().PaddingVertical(20).AlignCenter().Text("لا توجد حركات مسجلة لهذا الصنف خلال الفترة المحددة").Italic().FontColor(Colors.Grey.Medium);
            }
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
