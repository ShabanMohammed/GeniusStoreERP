using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace GeniusStoreERP.UI.Services;

public class PartnerSummaryReportDocument : IDocument
{
    private readonly List<PartnerAccountDto> _accounts;
    private readonly GeneralSettingsDto? _settings;

    public PartnerSummaryReportDocument(List<PartnerAccountDto> accounts, GeneralSettingsDto? settings)
    {
        _accounts = accounts;
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
                    reportColumn.Item().Text("ملخص حسابات الشركاء").FontSize(16).Bold().FontColor(Colors.Grey.Darken3);
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
                    columns.RelativeColumn(3f);   // Name
                    columns.RelativeColumn(1.5f); // Phone
                    columns.RelativeColumn(1f);   // Type
                    columns.RelativeColumn(1.5f); // Total Debit
                    columns.RelativeColumn(1.5f); // Total Credit
                    columns.RelativeColumn(1.5f); // Balance
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("م");
                    header.Cell().Element(HeaderStyle).Text("اسم الشريك");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("رقم الهاتف");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("النوع");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("مدين (عليه)");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("دائن (له)");
                    header.Cell().Element(HeaderStyle).AlignCenter().Text("الرصيد");

                    static IContainer HeaderStyle(IContainer container) => container.Background("#1E3A8A").Padding(6).DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(10));
                });

                int index = 1;
                foreach (var account in _accounts)
                {
                    table.Cell().Element(CellStyle).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(CellStyle).Text(account.Name);
                    table.Cell().Element(CellStyle).AlignCenter().Text(account.PhoneNumber ?? "-");
                    
                    var type = (account.IsSupplier && account.IsCustomer) ? "عميل ومورد" : 
                               account.IsSupplier ? "مورد" : 
                               account.IsCustomer ? "عميل" : "-";
                    table.Cell().Element(CellStyle).AlignCenter().Text(type);
                    
                    table.Cell().Element(CellStyle).AlignCenter().Text(account.TotalDebit.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignCenter().Text(account.TotalCredit.ToString("N2"));
                    
                    var balanceStyle = account.Balance >= 0 ? Colors.Green.Medium : Colors.Red.Medium;
                    table.Cell().Element(CellStyle).AlignCenter().Text(account.Balance.ToString("N2")).FontColor(balanceStyle).SemiBold();

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(6).DefaultTextStyle(x => x.FontSize(9));
                }
            });

            // Summary Totals
            column.Item().PaddingTop(15).Row(row =>
            {
                row.RelativeItem();
                row.RelativeItem().Column(totalColumn =>
                {
                    var totalBalance = _accounts.Sum(x => x.Balance);
                    totalColumn.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Black).Row(r =>
                    {
                        r.RelativeItem().Text("إجمالي الأرصدة:").Bold().FontSize(14);
                        r.RelativeItem().AlignLeft().Text($"{totalBalance:N2} {_settings?.CurrencySymbol ?? "EGP"}").Bold().FontSize(14).FontColor("#1E3A8A");
                    });
                });
            });
        });
    }
}
