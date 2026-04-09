using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeniusStoreERP.UI.Services;

public class InvoiceDocument : IDocument
{
    private readonly InvoiceDto _invoice;
    private readonly GeneralSettingsDto? _settings;

    public InvoiceDocument(InvoiceDto invoice, GeneralSettingsDto? settings)
    {
        _invoice = invoice;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Calibri")); // Calibri Font
            page.ContentFromRightToLeft(); // RTL for Arabic

            // Watermark for cancelled invoices
            if (_invoice.InvoiceStatusId == 2)
            {
                page.Background().AlignCenter().AlignMiddle().Rotate(-45).Text("ملغاة")
                    .FontSize(120).FontColor(Colors.Grey.Lighten3).SemiBold();
            }

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("صفحة ");
                x.CurrentPageNumber();
            });
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
                    
                    if (_settings != null)
                    {
                        if (!string.IsNullOrEmpty(_settings.Address))
                            infoColumn.Item().Text(_settings.Address).FontSize(10).FontColor(Colors.Grey.Darken1);
                        
                        if (!string.IsNullOrEmpty(_settings.Phone1))
                            infoColumn.Item().Text($"تليفون: {_settings.Phone1} {(_settings.Phone2 != null ? " - " + _settings.Phone2 : "")}").FontSize(10).FontColor(Colors.Grey.Darken1);
                        
                        if (!string.IsNullOrEmpty(_settings.TaxNumber))
                            infoColumn.Item().Text($"الرقم الضريبي: {_settings.TaxNumber}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    }
                });

                row.RelativeItem().AlignLeft().Column(reportColumn =>
                {
                    reportColumn.Item().Text(_invoice.TypeName).FontSize(16).Bold().FontColor(Colors.Grey.Darken3);
                    reportColumn.Item().Text($"رقم: {_invoice.InvoiceNumber}").FontSize(12);
                    reportColumn.Item().Text($"التاريخ: {_invoice.InvoiceDate:yyyy/MM/dd}").FontSize(12);
                    reportColumn.Item().Text($"الحالة: {_invoice.StatusName}").FontSize(11).FontColor(_invoice.InvoiceStatusId == 2 ? Colors.Red.Medium : Colors.Green.Medium);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor("#E5E7EB");
            
            column.Item().Row(row => {
                row.RelativeItem().Column(c => {
                    c.Item().Text("إلى السيد / السادة:").SemiBold().FontSize(12);
                    c.Item().Text(_invoice.PartnerName).FontSize(14);
                });
            });
            
            column.Item().PaddingBottom(10);
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
                    columns.ConstantColumn(25);   // Index
                    columns.RelativeColumn(2.5f);  // Product
                    columns.RelativeColumn(0.8f);  // Qty
                    columns.RelativeColumn(1.2f);  // Price
                    columns.RelativeColumn(1.2f);  // Total (Qty * Price)
                    columns.RelativeColumn(1f);    // Discount
                    columns.RelativeColumn(1f);    // Tax
                    columns.RelativeColumn(1.3f);  // Net
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("م");
                    header.Cell().Element(HeaderStyle).Text("البيان");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الكمية");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("السعر");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الإجمالي");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الخصم");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الضريبة");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الصافي");

                    static IContainer HeaderStyle(IContainer container) => container.Background("#1E3A8A").Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(9));
                });

                int index = 1;
                foreach (var item in _invoice.InvoiceItems)
                {
                    table.Cell().Element(CellStyle).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(CellStyle).Text(item.ProductName);
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.UnitPrice.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.LineTotal.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.DisCountAmount.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.TaxAmount.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.NetLineTotal.ToString("N2"));

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).DefaultTextStyle(x => x.FontSize(9));
                }
            });

            // Totals
            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem(); // Space to push totals left
                row.RelativeItem().Column(totalColumn =>
                {
                    totalColumn.Item().Row(r =>
                    {
                        r.RelativeItem().Text("إجمالي الأصناف:").FontSize(10);
                        r.RelativeItem().AlignLeft().Text(_invoice.TotalItemsAmount.ToString("N2")).FontSize(10);
                    });

                    if (_invoice.TotalItemsTax > 0)
                    {
                        totalColumn.Item().Row(r =>
                        {
                            r.RelativeItem().Text("إجمالي الضريبة:").FontSize(10);
                            r.RelativeItem().AlignLeft().Text(_invoice.TotalItemsTax.ToString("N2")).FontSize(10);
                        });
                    }

                    if (_invoice.TotalItemsDiscount > 0)
                    {
                        totalColumn.Item().Row(r =>
                        {
                            r.RelativeItem().Text("إجمالي الخصم:").FontSize(10);
                            r.RelativeItem().AlignLeft().Text(_invoice.TotalItemsDiscount.ToString("N2")).FontSize(10);
                        });
                    }

                    totalColumn.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Black).Row(r =>
                    {
                        r.RelativeItem().Text("الصافي النهائي:").Bold().FontSize(14);
                        r.RelativeItem().AlignLeft().Text($"{_invoice.FinalAmount:N2} {_settings?.CurrencySymbol ?? "EGP"}").Bold().FontSize(14).FontColor("#1E3A8A");
                    });
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
