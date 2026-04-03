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

    public byte[] GeneratePdf(InvoiceDto invoice)
    {
        var document = new InvoiceDocument(invoice);
        return document.GeneratePdf();
    }

    public byte[] GenerateExcel(InvoiceDto invoice)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoice");

        // Simple Layout
        worksheet.Cell(1, 1).Value = "اسم النظام: Genius Store ERP";
        worksheet.Cell(2, 1).Value = "الفاتورة: " + invoice.TypeName;
        worksheet.Cell(2, 2).Value = "رقم الفاتورة: " + invoice.InvoiceNumber;
        worksheet.Cell(2, 3).Value = "التاريخ: " + invoice.InvoiceDate.ToString("yyyy-MM-dd");

        worksheet.Cell(4, 1).Value = "السيد/ " + invoice.PartnerName;

        // Headers
        var headers = new[] { "الصنف", "الكمية", "السعر", "الإجمالي" };
        for (int i = 0; i < headers.Length; i++)
        {
            worksheet.Cell(6, i + 1).Value = headers[i];
            worksheet.Cell(6, i + 1).Style.Font.Bold = true;
            worksheet.Cell(6, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        // Data
        int row = 7;
        foreach (var item in invoice.InvoiceItems)
        {
            worksheet.Cell(row, 1).Value = item.ProductName;
            worksheet.Cell(row, 2).Value = item.Quantity;
            worksheet.Cell(row, 3).Value = item.UnitPrice;
            worksheet.Cell(row, 4).Value = item.NetLineTotal;
            row++;
        }

        // Totals
        row++;
        worksheet.Cell(row, 3).Value = "إجمالي الأصناف:";
        worksheet.Cell(row, 4).Value = invoice.TotalItemsAmount;

        row++;
        worksheet.Cell(row, 3).Value = "الضريبة:";
        worksheet.Cell(row, 4).Value = invoice.TotalItemsTax;

        row++;
        worksheet.Cell(row, 3).Value = "الخصم:";
        worksheet.Cell(row, 4).Value = invoice.TotalItemsDiscount;

        row++;
        worksheet.Cell(row, 3).Value = "الصافي النهائي:";
        worksheet.Cell(row, 3).Style.Font.Bold = true;
        worksheet.Cell(row, 4).Value = invoice.FinalAmount;
        worksheet.Cell(row, 4).Style.Font.Bold = true;

        worksheet.Columns().AdjustToContents();
        worksheet.RightToLeft = true; // RTL for Arabic

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GenerateWord(InvoiceDto invoice)
    {
        using var stream = new MemoryStream();
        using var document = DocX.Create(stream);

        // Header
        document.InsertParagraph("Genius Store ERP").FontSize(18).Bold().Color(System.Drawing.Color.Blue).Alignment = Alignment.center;
        document.InsertParagraph("نظام إدارة المخازن والمبيعات").FontSize(10).Alignment = Alignment.center;

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
        var itemsTable = document.AddTable(invoice.InvoiceItems.Count + 1, 4);
        itemsTable.Alignment = Alignment.right;
        itemsTable.Rows[0].Cells[0].Paragraphs[0].Append("الصنف").Bold();
        itemsTable.Rows[0].Cells[1].Paragraphs[0].Append("الكمية").Bold();
        itemsTable.Rows[0].Cells[2].Paragraphs[0].Append("السعر").Bold();
        itemsTable.Rows[0].Cells[3].Paragraphs[0].Append("الإجمالي").Bold();

        int rowIdx = 1;
        foreach (var item in invoice.InvoiceItems)
        {
            itemsTable.Rows[rowIdx].Cells[0].Paragraphs[0].Append(item.ProductName);
            itemsTable.Rows[rowIdx].Cells[1].Paragraphs[0].Append(item.Quantity.ToString());
            itemsTable.Rows[rowIdx].Cells[2].Paragraphs[0].Append(item.UnitPrice.ToString("N2"));
            itemsTable.Rows[rowIdx].Cells[3].Paragraphs[0].Append(item.NetLineTotal.ToString("N2"));
            rowIdx++;
        }
        document.InsertTable(itemsTable);

        document.InsertParagraph().SpacingAfter(20);

        // Totals
        document.InsertParagraph($"إجمالي الأصناف: {invoice.TotalItemsAmount:N2}").Alignment = Alignment.left;
        document.InsertParagraph($"الضريبة: {invoice.TotalItemsTax:N2}").Alignment = Alignment.left;
        document.InsertParagraph($"الخصم: {invoice.TotalItemsDiscount:N2}").Alignment = Alignment.left;
        document.InsertParagraph($"الصافي النهائي: {invoice.FinalAmount:N2}").Bold().FontSize(14).Alignment = Alignment.left;

        document.Save();
        return stream.ToArray();
    }
}
