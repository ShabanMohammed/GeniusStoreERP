using ClosedXML.Excel;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;
using System.IO;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace GeniusStoreERP.UI.Services;

public class InvoiceReportService : IInvoiceReportService
{
    public InvoiceReportService()
    {
        // Initializing QuestPDF License (Community is free for certain uses)
        QuestPDF.Settings.License = LicenseType.Community;

        // Register Cairo font for QuestPDF
        try
        {
            var fontPathRegular = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fonts", "Cairo-Regular.ttf");
            var fontPathBold = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fonts", "Cairo-Bold.ttf");
            
            if (File.Exists(fontPathRegular))
                QuestPDF.Drawing.FontManager.RegisterFont(File.OpenRead(fontPathRegular));
            if (File.Exists(fontPathBold))
                QuestPDF.Drawing.FontManager.RegisterFont(File.OpenRead(fontPathBold));
        }
        catch { /* Font registration might fail if already registered or files missing */ }
    }

    public byte[] GeneratePdf(InvoiceDto invoice, GeneralSettingsDto? settings)
    {
        var document = new InvoiceDocument(invoice, settings);
        return document.GeneratePdf();
    }

    public byte[] GenerateExcel(InvoiceDto invoice, GeneralSettingsDto? settings)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoice");

        var companyName = settings?.CompanyName ?? "Genius Store ERP";
        // Simple Layout
        worksheet.Cell(1, 1).Value = "اسم الشركة: " + companyName;
        if (settings != null)
        {
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(2, 1).Value = "العنوان: " + settings.Address;
            worksheet.Cell(3, 1).Value = "الهاتف: " + settings.Phone1;
        }

        worksheet.Cell(5, 1).Value = "الفاتورة: " + invoice.TypeName;
        worksheet.Cell(5, 2).Value = "رقم الفاتورة: " + invoice.InvoiceNumber;
        worksheet.Cell(5, 3).Value = "التاريخ: " + invoice.InvoiceDate.ToString("yyyy-MM-dd");
        worksheet.Cell(5, 4).Value = "الحالة: " + invoice.StatusName;

        worksheet.Cell(7, 1).Value = "السيد/ " + invoice.PartnerName;

        // Headers
        var headers = new[] { "الصنف", "الكمية", "السعر", "الإجمالي", "الخصم", "الضريبة", "الصافي" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(9, i + 1).Value = headers[i];
            worksheet.Cell(9, i + 1).Style.Font.Bold = true;
            worksheet.Cell(9, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data
        int row = 10;
        foreach (var item in invoice.InvoiceItems)
        {
            worksheet.Cell(row, 1).Value = item.ProductName;
            worksheet.Cell(row, 2).Value = item.Quantity;
            worksheet.Cell(row, 3).Value = item.UnitPrice;
            worksheet.Cell(row, 4).Value = item.LineTotal;
            worksheet.Cell(row, 5).Value = item.DisCountAmount;
            worksheet.Cell(row, 6).Value = item.TaxAmount;
            worksheet.Cell(row, 7).Value = item.NetLineTotal;
            row++;
        }

        // Totals
        row++;
        worksheet.Cell(row, 6).Value = "إجمالي الأصناف:";
        worksheet.Cell(row, 7).Value = invoice.TotalItemsAmount;

        row++;
        worksheet.Cell(row, 6).Value = "إجمالي الضريبة:";
        worksheet.Cell(row, 7).Value = invoice.TotalItemsTax;

        row++;
        worksheet.Cell(row, 6).Value = "إجمالي الخصم:";
        worksheet.Cell(row, 7).Value = invoice.TotalItemsDiscount;

        row++;
        worksheet.Cell(row, 6).Value = "الصافي النهائي:";
        worksheet.Cell(row, 6).Style.Font.Bold = true;
        worksheet.Cell(row, 7).Value = invoice.FinalAmount;
        worksheet.Cell(row, 7).Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();
        worksheet.RightToLeft = true; // RTL for Arabic

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateWord(InvoiceDto invoice, GeneralSettingsDto? settings)
    {
        using var stream = new MemoryStream();
        using var document = DocX.Create(stream);

        var companyName = settings?.CompanyName ?? "Genius Store ERP";
        // Header
        document.InsertParagraph(companyName).FontSize(18).Bold().Color(System.Drawing.Color.Blue).Alignment = Alignment.center;
        if (settings != null)
        {
            document.InsertParagraph(settings.Address ?? "").FontSize(10).Alignment = Alignment.center;
            document.InsertParagraph(settings.Phone1 ?? "").FontSize(10).Alignment = Alignment.center;
        }
        else
        {
            document.InsertParagraph("نظام إدارة المخازن والمبيعات").FontSize(10).Alignment = Alignment.center;
        }

        document.InsertParagraph().SpacingAfter(20);

        // Metadata
        var tableMeta = document.AddTable(2, 2);
        tableMeta.Alignment = Alignment.right;
        tableMeta.Rows[0].Cells[0].Paragraphs[0].Append("النوع: " + invoice.TypeName);
        tableMeta.Rows[0].Cells[1].Paragraphs[0].Append("رقم الفاتورة: " + invoice.InvoiceNumber);
        tableMeta.Rows[1].Cells[0].Paragraphs[0].Append("التاريخ: " + invoice.InvoiceDate.ToString("yyyy-MM-dd"));
        tableMeta.Rows[1].Cells[1].Paragraphs[0].Append("العميل/المورد: " + invoice.PartnerName);
        document.InsertTable(tableMeta);

        document.InsertParagraph().SpacingAfter(20);

        // Items Table
        var itemsTable = document.AddTable(invoice.InvoiceItems.Count + 1, 7);
        itemsTable.Alignment = Alignment.right;
        itemsTable.Rows[0].Cells[0].Paragraphs[0].Append("الصنف").Bold();
        itemsTable.Rows[0].Cells[1].Paragraphs[0].Append("الكمية").Bold();
        itemsTable.Rows[0].Cells[2].Paragraphs[0].Append("السعر").Bold();
        itemsTable.Rows[0].Cells[3].Paragraphs[0].Append("الإجمالي").Bold();
        itemsTable.Rows[0].Cells[4].Paragraphs[0].Append("الخصم").Bold();
        itemsTable.Rows[0].Cells[5].Paragraphs[0].Append("الضريبة").Bold();
        itemsTable.Rows[0].Cells[6].Paragraphs[0].Append("الصافي").Bold();

        int rowIdx = 1;
        foreach (var item in invoice.InvoiceItems)
        {
            itemsTable.Rows[rowIdx].Cells[0].Paragraphs[0].Append(item.ProductName);
            itemsTable.Rows[rowIdx].Cells[1].Paragraphs[0].Append(item.Quantity.ToString());
            itemsTable.Rows[rowIdx].Cells[2].Paragraphs[0].Append(item.UnitPrice.ToString("N2"));
            itemsTable.Rows[rowIdx].Cells[3].Paragraphs[0].Append(item.LineTotal.ToString("N2"));
            itemsTable.Rows[rowIdx].Cells[4].Paragraphs[0].Append(item.DisCountAmount.ToString("N2"));
            itemsTable.Rows[rowIdx].Cells[5].Paragraphs[0].Append(item.TaxAmount.ToString("N2"));
            itemsTable.Rows[rowIdx].Cells[6].Paragraphs[0].Append(item.NetLineTotal.ToString("N2"));
            rowIdx++;
        }
        document.InsertTable(itemsTable);

        document.InsertParagraph().SpacingAfter(20);

        // Totals
        document.InsertParagraph($"إجمالي الأصناف: {invoice.TotalItemsAmount:N2}").Alignment = Alignment.left;
        document.InsertParagraph($"إجمالي الضريبة: {invoice.TotalItemsTax:N2}").Alignment = Alignment.left;
        document.InsertParagraph($"إجمالي الخصم: {invoice.TotalItemsDiscount:N2}").Alignment = Alignment.left;
        document.InsertParagraph($"الصافي النهائي: {invoice.FinalAmount:N2} {settings?.CurrencySymbol}").Bold().FontSize(14).Alignment = Alignment.left;

        document.Save();
        return stream.ToArray();
    }
}
