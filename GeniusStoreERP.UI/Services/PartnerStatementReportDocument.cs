using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace GeniusStoreERP.UI.Services;

public class PartnerStatementReportDocument : IDocument
{
    private readonly PartnerStatementDto _statement;
    private readonly GeneralSettingsDto? _settings;

    public PartnerStatementReportDocument(PartnerStatementDto statement, GeneralSettingsDto? settings)
    {
        _statement = statement;
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
                    reportColumn.Item().Text("كشف حساب شريك").FontSize(16).Bold().FontColor(Colors.Grey.Darken3);
                    reportColumn.Item().Text($"تاريخ التقرير: {System.DateTime.Now:yyyy/MM/dd}").FontSize(12);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor("#E5E7EB");

            column.Item().Row(row => {
                row.RelativeItem().Column(c => {
                    c.Item().Text("اسم الشريك:").SemiBold().FontSize(12);
                    c.Item().Text(_statement.Partner.Name).FontSize(14).SemiBold().FontColor("#1E3A8A");
                    if (!string.IsNullOrEmpty(_statement.Partner.PhoneNumber))
                        c.Item().Text($"رقم الهاتف: {_statement.Partner.PhoneNumber}").FontSize(10);
                });

                row.RelativeItem().AlignLeft().Column(c => {
                    c.Item().Text("الرصيد الافتتاحي:").SemiBold().FontSize(12);
                    c.Item().Text(_statement.OpeningBalance.ToString("N2")).FontSize(14).SemiBold();
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
                    columns.ConstantColumn(80);   // Date
                    columns.RelativeColumn(1.5f); // Type
                    columns.RelativeColumn(1f);   // Reference
                    columns.RelativeColumn(2f);   // Remarks
                    columns.RelativeColumn(1f);   // Debit
                    columns.RelativeColumn(1f);   // Credit
                    columns.RelativeColumn(1.2f); // Balance
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("التاريخ");
                    header.Cell().Element(HeaderStyle).Text("نوع العملية");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("رقم المرجع");
                    header.Cell().Element(HeaderStyle).Text("ملاحظات");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("مدين (عليه)");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("دائن (له)");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الرصيد");

                    static IContainer HeaderStyle(IContainer container) => container.Background("#1E3A8A").Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(10));
                });

                foreach (var item in _statement.Items)
                {
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Date.ToString("yyyy/MM/dd"));
                    table.Cell().Element(CellStyle).Text(item.TransactionType);
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.ReferenceNumber ?? "-");
                    table.Cell().Element(CellStyle).Text(item.Remarks ?? "-");
                    
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Debit > 0 ? item.Debit.ToString("N2") : "-");
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Credit > 0 ? item.Credit.ToString("N2") : "-");
                    
                    var balanceStyle = item.RunningBalance >= 0 ? Colors.Green.Medium : Colors.Red.Medium;
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.RunningBalance.ToString("N2")).FontColor(balanceStyle).SemiBold();

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).DefaultTextStyle(x => x.FontSize(9));
                }
            });

            // Summary Totals
            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem();
                row.RelativeItem().Column(totalColumn =>
                {
                    totalColumn.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Black).Row(r =>
                    {
                        r.RelativeItem().Text("الرصيد الختامي:").Bold().FontSize(14);
                        r.RelativeItem().AlignLeft().Text($"{_statement.ClosingBalance:N2} {_settings?.CurrencySymbol ?? "EGP"}").Bold().FontSize(14).FontColor("#1E3A8A");
                    });
                });
            });
        });
    }
}
