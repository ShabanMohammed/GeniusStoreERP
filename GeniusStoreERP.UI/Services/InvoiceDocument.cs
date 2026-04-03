using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeniusStoreERP.UI.Services;

public class InvoiceDocument : IDocument
{
    private readonly InvoiceDto _invoice;

    public InvoiceDocument(InvoiceDto invoice)
    {
        _invoice = invoice;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Cairo")); // Use Cairo Font
            page.ContentFromRightToLeft(); // RTL for Arabic

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
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Genius Store ERP").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                column.Item().Text("نظام إدارة المخازن والمبيعات").FontSize(10).FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem().AlignLeft().Column(column =>
            {
                column.Item().Text(_invoice.TypeName).FontSize(16).SemiBold();
                column.Item().Text($"رقم الفاتورة: {_invoice.InvoiceNumber}");
                column.Item().Text($"التاريخ: {_invoice.InvoiceDate:yyyy-MM-dd}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Partner Info
            column.Item().BorderBottom(1).PaddingBottom(5).Row(row =>
            {
                row.RelativeItem().Text(x =>
                {
                    x.Span("السيد/ ").SemiBold();
                    x.Span(_invoice.PartnerName);
                });
            });

            // Table
            column.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Product
                    columns.RelativeColumn(1); // Quantity
                    columns.RelativeColumn(1.5f); // Price
                    columns.RelativeColumn(1.5f); // Total
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("الصنف").SemiBold();
                    header.Cell().Element(CellStyle).Text("الكمية").SemiBold();
                    header.Cell().Element(CellStyle).Text("السعر").SemiBold();
                    header.Cell().Element(CellStyle).Text("الإجمالي").SemiBold();

                    static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                foreach (var item in _invoice.InvoiceItems)
                {
                    table.Cell().Element(CellStyle).Text(item.ProductName);
                    table.Cell().Element(CellStyle).Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).Text(item.UnitPrice.ToString("N2"));
                    table.Cell().Element(CellStyle).Text(item.NetLineTotal.ToString("N2"));

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Grey.Lighten2);
                }
            });

            // Totals
            column.Item().AlignLeft().PaddingTop(10).Column(sub =>
            {
                sub.Item().Row(row =>
                {
                    row.RelativeItem().Text("إجمالي الأصناف: ");
                    row.RelativeItem().AlignLeft().Text(_invoice.TotalItemsAmount.ToString("N2"));
                });
                if (_invoice.TotalItemsTax > 0)
                {
                    sub.Item().Row(row =>
                    {
                        row.RelativeItem().Text("الضريبة: ");
                        row.RelativeItem().AlignLeft().Text(_invoice.TotalItemsTax.ToString("N2"));
                    });
                }
                if (_invoice.TotalItemsDiscount > 0)
                {
                    sub.Item().Row(row =>
                    {
                        row.RelativeItem().Text("الخصم: ");
                        row.RelativeItem().AlignLeft().Text(_invoice.TotalItemsDiscount.ToString("N2"));
                    });
                }
                sub.Item().BorderTop(1).PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text("الصافي النهائي: ").SemiBold().FontSize(14);
                    row.RelativeItem().AlignLeft().Text(_invoice.FinalAmount.ToString("N2")).SemiBold().FontSize(14);
                });
            });
        });
    }
}
