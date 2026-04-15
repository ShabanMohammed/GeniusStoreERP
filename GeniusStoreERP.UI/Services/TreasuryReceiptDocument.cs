using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Domain.Entities.Finances;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GeniusStoreERP.UI.Services;

public class TreasuryReceiptDocument : IDocument
{
    private readonly TreasuryTransactionDto _transaction;
    private readonly GeneralSettingsDto? _settings;
    private readonly string _amountInWords;

    public TreasuryReceiptDocument(TreasuryTransactionDto transaction, GeneralSettingsDto? settings)
    {
        _transaction = transaction;
        _settings = settings;
        _amountInWords = CurrencyToWordsHelper.ConvertToArabic(transaction.Amount, settings?.CurrencySymbol ?? "جنيه");
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Calibri"));
            page.ContentFromRightToLeft();

            page.Content().Column(column =>
            {
                // Copy 1: Original
                column.Item().Element(c => ComposeReceipt(c, "أصل الإيصال", false));
                
                // Divider
                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // Copy 2: Accounting Copy
                column.Item().Element(c => ComposeReceipt(c, "صورة - مراجعة الحسابات", true));

                // Divider
                column.Item().PaddingVertical(2).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // Copy 3: Customer Copy
                column.Item().Element(c => ComposeReceipt(c, "صورة - العميل", true));
            });
        });
    }

    private void ComposeReceipt(IContainer container, string title, bool isCopy)
    {
        var receipt = container.Height(8, Unit.Centimetre).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8);

        if (isCopy)
        {
            receipt.Layers(layers => 
            {
                layers.Layer().AlignCenter().AlignMiddle().Rotate(-20).Text("صــورة")
                    .FontSize(60).FontColor(Colors.Grey.Lighten4).SemiBold();
                
                layers.PrimaryLayer().Element(c => ComposeReceiptContent(c));
            });
        }
        else
        {
            ComposeReceiptContent(receipt);
        }

        void ComposeReceiptContent(IContainer c)
        {
            c.Column(column =>
            {
                // Header: Company Logo & Info
                column.Item().Row(row =>
                {
                    // Logo
                    if (_settings?.Logo != null && _settings.Logo.Length > 0)
                    {
                        row.ConstantItem(60).Width(50).Height(50).Image(_settings.Logo);
                    }

                    // Company Details
                    row.RelativeItem().PaddingRight(10).Column(details =>
                    {
                        details.Item().Text(_settings?.CompanyName ?? "Genius Store ERP").FontSize(14).Bold().FontColor("#1E3A8A");
                        if (!string.IsNullOrEmpty(_settings?.Address))
                            details.Item().Text(_settings.Address).FontSize(8).FontColor(Colors.Grey.Darken1);
                        if (!string.IsNullOrEmpty(_settings?.Phone1))
                            details.Item().Text($"تليفون: {_settings.Phone1}").FontSize(8).FontColor(Colors.Grey.Darken1);
                    });

                    // Receipt Title & Meta
                    row.RelativeItem().AlignLeft().Column(meta =>
                    {
                        meta.Item().Text(title).FontSize(12).Bold().FontColor(Colors.Grey.Darken3);
                        meta.Item().Text($"رقم المعاملة: {_transaction.Id}").FontSize(9);
                        meta.Item().Text($"التاريخ: {_transaction.TransactionDate:yyyy/MM/dd HH:mm}").FontSize(9);
                    });
                });

                column.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);

                // Body
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(body =>
                    {
                        body.Item().Text(t =>
                        {
                            if (_transaction.Type == TreasuryTransactionType.CashIn)
                            {
                                t.Span("وصلنا من السيد / ").FontSize(11);
                                t.Span(_transaction.PartnerName ?? "-----------").FontSize(12).Bold();
                            }
                            else
                            {
                                t.Span("صرفنا للسيد / ").FontSize(11);
                                t.Span(_transaction.PartnerName ?? "-----------").FontSize(12).Bold();
                            }
                        });

                        body.Item().PaddingTop(5).Text(t =>
                        {
                            var wording = _transaction.Type == TreasuryTransactionType.CashIn ? "وذلك عـن مبلغ وقـدره: " : "مبلغاً وقدره: ";
                            t.Span(wording).FontSize(11);
                            t.Span(_transaction.Amount.ToString("N2")).FontSize(12).Bold();
                            t.Span($" {_settings?.CurrencySymbol ?? "EGP"}").FontSize(10);
                        });

                        body.Item().PaddingTop(2).Text(t =>
                        {
                            t.Span("( ").FontSize(10);
                            t.Span(_amountInWords).FontSize(11).Medium().Italic();
                            t.Span(" )").FontSize(10);
                        });

                        body.Item().PaddingTop(5).Text(t =>
                        {
                            t.Span("وذلك بخصوص: ").FontSize(11);
                            t.Span(_transaction.Notes ?? "تحويل مالي").FontSize(11);
                        });
                    });
                });

                // Footer
                column.Item().AlignBottom().Row(row =>
                {
                    row.RelativeItem().Text("توقيع المستلم").FontSize(10).AlignCenter();
                    row.RelativeItem().PaddingRight(50); // Empty space
                    row.RelativeItem().Text("الختم").FontSize(10).AlignCenter();
                });
            });
        }
    }
}
