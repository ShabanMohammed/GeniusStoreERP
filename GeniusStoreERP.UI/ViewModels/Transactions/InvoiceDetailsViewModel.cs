using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Application.Transactions.Commands.VoidInvoiceByReverse;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using GeniusStoreERP.Application.Common.Interfaces;

namespace GeniusStoreERP.UI.ViewModels.Transactions;

public class InvoiceDetailsViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly INavigationService _navigationService;
    private readonly IInvoiceReportService _reportService;

    private InvoiceDto? _invoice;
    public InvoiceDto? Invoice
    {
        get => _invoice;
        set => SetProperty(ref _invoice, value);
    }

    public bool CanVoid => Invoice != null && Invoice.InvoiceStatusId != 2; // Not already voided

    public ICommand ExportPdfCommand { get; }
    public ICommand ExportExcelCommand { get; }
    public ICommand ExportWordCommand { get; }
    public ICommand VoidCommand { get; }
    public ICommand BackCommand { get; }

    public InvoiceDetailsViewModel(IMediator mediator, INavigationService navigationService, IInvoiceReportService reportService)
    {
        _mediator = mediator;
        _navigationService = navigationService;
        _reportService = reportService;

        ExportPdfCommand = new RelayCommand(_ => ExportToPdf());
        ExportExcelCommand = new RelayCommand(_ => ExportToExcel());
        ExportWordCommand = new RelayCommand(_ => ExportToWord());
        VoidCommand = new AsyncRelayCommand(async (p, _) => await VoidInvoice());
        BackCommand = new RelayCommand(_ => _navigationService.NavigateTo<InvoiceListViewModel>(Invoice?.InvoiceTypeId ?? 1));
    }

    public override void Initialize(object? parameter)
    {
        if (parameter is InvoiceDto invoice)
        {
            Invoice = invoice;
            OnPropertyChanged(nameof(CanVoid));
        }
    }

    private void ExportToPdf()
    {
        if (Invoice == null) return;
        
        var dialog = new SaveFileDialog
        {
            Filter = "PDF Files (*.pdf)|*.pdf",
            FileName = $"Invoice_{Invoice.InvoiceNumber}.pdf"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var bytes = _reportService.GeneratePdf(Invoice);
                File.WriteAllBytes(dialog.FileName, bytes);
                MessageBoxService.ShowSuccess("تم تصدير ملف PDF بنجاح");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"فشل التصدير: {ex.Message}");
            }
        }
    }

    private void ExportToExcel()
    {
        if (Invoice == null) return;

        var dialog = new SaveFileDialog
        {
            Filter = "Excel Files (*.xlsx)|*.xlsx",
            FileName = $"Invoice_{Invoice.InvoiceNumber}.xlsx"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var bytes = _reportService.GenerateExcel(Invoice);
                File.WriteAllBytes(dialog.FileName, bytes);
                MessageBoxService.ShowSuccess("تم تصدير ملف Excel بنجاح");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"فشل التصدير: {ex.Message}");
            }
        }
    }

    private void ExportToWord()
    {
        if (Invoice == null) return;

        var dialog = new SaveFileDialog
        {
            Filter = "Word Files (*.docx)|*.docx",
            FileName = $"Invoice_{Invoice.InvoiceNumber}.docx"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var bytes = _reportService.GenerateWord(Invoice);
                File.WriteAllBytes(dialog.FileName, bytes);
                MessageBoxService.ShowSuccess("تم تصدير ملف Word بنجاح");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"فشل التصدير: {ex.Message}");
            }
        }
    }

    private async Task VoidInvoice()
    {
        if (Invoice == null) return;

        var result = MessageBoxService.ShowConfirmation($"هل تريد بالتأكيد إلغاء الفاتورة رقم {Invoice.InvoiceNumber}؟\nسيتم عكس جميع تأثيرات هذه الفاتورة.");
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                var command = new VoidInvoiceByReverseCommand(Invoice.Id);
                await _mediator.Send(command);
                
                MessageBoxService.ShowSuccess("تم إلغاء الفاتورة بنجاح.");
                
                // Refresh the current view to show the "Voided" status
                // Since DTOs are immutable records in this project, we might need to reload or manually update
                // For now, let's navigate back to the list
                _navigationService.NavigateTo<InvoiceListViewModel>(Invoice.InvoiceTypeId);
            }
            catch (BusinessException ex)
            {
                MessageBoxService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"حدث خطأ أثناء الإلغاء: {ex.Message}");
            }
        }
    }
}
