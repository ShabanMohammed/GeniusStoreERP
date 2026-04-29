using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace GeniusStoreERP.UI.Services;

public class DebtAgingReportDocument : IDocument
{
    private readonly List<DebtAgingDto> _agingData;
    private readonly GeneralSettingsDto? _settings;

    public DebtAgingReportDocument(List<DebtAgingDto> agingData, GeneralSettingsDto? settings)
    {
        _agingData = agingData;
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
                    }
                });

                row.RelativeItem().AlignLeft().Column(reportColumn =>
                {
                    reportColumn.Item().Text("تقرير أعمار الديون").FontSize(16).Bold().FontColor(Colors.Grey.Darken3);
                    reportColumn.Item().Text($"تاريخ التقرير: {System.DateTime.Now:yyyy/MM/dd}").FontSize(12);
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
                    columns.ConstantColumn(30);   // Index
                    columns.RelativeColumn(2.5f); // Name
                    columns.RelativeColumn(1.2f); // Current
                    columns.RelativeColumn(1.2f); // 31-60
                    columns.RelativeColumn(1.2f); // 61-90
                    columns.RelativeColumn(1.2f); // 90+
                    columns.RelativeColumn(1.5f); // Total
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("م");
                    header.Cell().Element(HeaderStyle).Text("اسم الشريك");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("0-30 يوم");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("31-60 يوم");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("61-90 يوم");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("+90 يوم");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الإجمالي");

                    static IContainer HeaderStyle(IContainer container) => container.Background("#1E3A8A").Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(9));
                });

                int index = 1;
                foreach (var row in _agingData)
                {
                    table.Cell().Element(CellStyle).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(CellStyle).Text(row.PartnerName);
                    table.Cell().Element(CellStyle).AlignCenter().Text(row.Current.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(row.ThirtyToSixty.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(row.SixtyToNinety.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(row.OverNinety.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(row.TotalBalance.ToString("N2")).SemiBold();

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).DefaultTextStyle(x => x.FontSize(8.5f));
                }
            });

            // Summary Totals
            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem();
                row.RelativeItem().Column(totalColumn =>
                {
                    var grandTotal = _agingData.Sum(x => x.TotalBalance);
                    totalColumn.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Black).Row(r =>
                    {
                        r.RelativeItem().Text("إجمالي المديونيات:").Bold().FontSize(12);
                        r.RelativeItem().AlignLeft().Text($"{grandTotal:N2} {_settings?.CurrencySymbol ?? "EGP"}").Bold().FontSize(12).FontColor("#1E3A8A");
                    });
                });
            });
        });
    }
}
