using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Application.Transactions.Commands.VoidInvoiceByReverse;
using GeniusStoreERP.Application.Transactions.Queries.GetInvoicesByType;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.UI.ViewModels;
using GeniusStoreERP.UI.Views;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;

using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;
using System.Threading.Tasks;

namespace GeniusStoreERP.UI.ViewModels.Transactions;

public class InvoiceListViewModel : BaseViewModel
{

    private readonly IMediator _mediator;
    private readonly IInvoiceReportService _reportService;
    private readonly IServiceProvider _serviceProvider;

    private INavigationService _navigationService;

    public int InvoiceTypeId { get; private set; }

    private string pageTitle;

    public string PageTitle
    {
        get { return pageTitle; }
        private set => SetProperty(ref pageTitle, value);
    }

    private ObservableCollection<InvoiceDto> invoiceList;

    public ObservableCollection<InvoiceDto> InvoiceList
    {
        get { return invoiceList; }

        set => SetProperty(ref invoiceList, value);
    }



    private string searchText;

    public string SearchText
    {
        get { return searchText; }

        set
        {
            if (SetProperty(ref searchText, value))
            {
                CurrentPage = 1;
                _ = LoadInvoices();
            }
        }
    }

    private bool isLoading;

    public bool IsLoading
    {
        get { return isLoading; }
        set => SetProperty(ref isLoading, value);
    }

    public bool IsSalesMode => InvoiceTypeId == 1 || InvoiceTypeId == 3; // مبيعات أو مرتجع مبيعات
    public bool IsPurchaseMode => InvoiceTypeId == 2 || InvoiceTypeId == 4;

    private int _pageSize = 10;
    private int _currentPage = 1;
    private int _totalItems = 0;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value))
            {
                OnPropertyChanged(nameof(TotalPages));
                CurrentPage = 1;
                _ = LoadInvoices();
            }
        }
    }

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
            {
                _ = LoadInvoices();
            }
        }
    }

    public int TotalItems
    {
        get => _totalItems;
        set
        {
            if (SetProperty(ref _totalItems, value))
            {
                OnPropertyChanged(nameof(TotalPages));
            }
        }
    }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / (PageSize > 0 ? PageSize : 10));

    public ICommand AddInvoiceCommand { get; }

    public ICommand PrintInvoiceCommand { get; }
    public ICommand ViewDetailsCommand { get; }
    public ICommand DeleteInvoiceCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand IncreasePageSizeCommand { get; }
    public ICommand DecreasePageSizeCommand { get; }

    public InvoiceListViewModel(IMediator mediator, INavigationService navigationService,
        IInvoiceReportService reportService, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _navigationService = navigationService;
        _reportService = reportService;
        _serviceProvider = serviceProvider;

        pageTitle = string.Empty;
        invoiceList = new ObservableCollection<InvoiceDto>();
        searchText = string.Empty;

        AddInvoiceCommand = new RelayCommand(_ => NavigateToEditor(null));
        PrintInvoiceCommand = new AsyncRelayCommand(async (param, _) => await PrintInvoice(param as InvoiceDto));
        ViewDetailsCommand = new RelayCommand(param => NavigateToDetails(param as InvoiceDto));
        DeleteInvoiceCommand = new AsyncRelayCommand(async (param, _) => await DeleteInvoice(param as InvoiceDto));

        NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPages) CurrentPage++; }, _ => CurrentPage < TotalPages);
        PreviousPageCommand = new RelayCommand(_ => { if (CurrentPage > 1) CurrentPage--; }, _ => CurrentPage > 1);
        FirstPageCommand = new RelayCommand(_ => CurrentPage = 1, _ => CurrentPage > 1);
        LastPageCommand = new RelayCommand(_ => { if (TotalPages > 0) CurrentPage = TotalPages; }, _ => CurrentPage < TotalPages);

        IncreasePageSizeCommand = new RelayCommand(_ => PageSize += 1);
        DecreasePageSizeCommand = new RelayCommand(_ => { if (PageSize > 1) PageSize -= 1; });
    }

    public override async void Initialize(object? parameter)
    {
        if (parameter is int typeId)
        {
            InvoiceTypeId = typeId;
            PageTitle = typeId switch
            {
                1 => "قائمة فواتير المبيعات",
                2 => "قائمة فواتير المشتريات",
                3 => "مرتجع المبيعات",
                4 => "مرتجع المشتريات",
                _ => "الفواتير"
            };

            // إبلاغ الواجهة بتحديث خصائص الـ Mode
            OnPropertyChanged(nameof(IsSalesMode));
            OnPropertyChanged(nameof(IsPurchaseMode));

            await LoadInvoices();
        }
    }


    public async Task LoadInvoices()
    {
        try
        {
            IsLoading = true;
            var result = await _mediator.Send(new GetInvoicesByTypeQuery(InvoiceTypeId, CurrentPage, PageSize));

            InvoiceList = new ObservableCollection<InvoiceDto>(result.Items);
            TotalItems = result.TotalCount;

            if (CurrentPage > TotalPages && TotalPages > 0)
                CurrentPage = TotalPages;

        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteInvoice(InvoiceDto? invoice)
    {
        if (invoice == null) return;

        if (invoice.InvoiceStatusId == 2) // الملغاة مسبقاً
        {
            MessageBoxService.ShowWarning("هذه الفاتورة ملغاة بالفعل.");
            return;
        }

        var result = MessageBoxService.ShowConfirmation($"هل تريد بالتأكيد إلغاء الفاتورة رقم {invoice.InvoiceNumber}؟\nسيتم عكس جميع تأثيرات هذه الفاتورة.");
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                IsLoading = true;
                var command = new VoidInvoiceByReverseCommand(invoice.Id);
                await _mediator.Send(command);
                
                MessageBoxService.ShowSuccess("تم إلغاء الفاتورة بنجاح.");
                await LoadInvoices(); // تحديث القائمة
            }
            catch (NotFoundException ex)
            {
                MessageBoxService.ShowError($"{ex.Message}الفاتورة غير موجودة");
            }
            catch (BusinessException ex)
            {
                MessageBoxService.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"حدث خطأ غير متوقع: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task PrintInvoice(InvoiceDto? invoice)
    {
        if (invoice == null) return;

        try
        {
            var settings = await _mediator.Send(new GetGeneralSettingsQuery());
            // توليد ملف PDF للفاتورة
            var pdfData = _reportService.GeneratePdf(invoice, settings);

            // الحصول على ReportPreviewWindow من DI
            var previewWindow = _serviceProvider.GetRequiredService<ReportPreviewWindow>();
            var previewVm = previewWindow.DataContext as ReportPreviewViewModel;

            if (previewVm != null)
            {
                var title = $"فاتورة رقم {invoice.InvoiceNumber} - {invoice.PartnerName}";
                var info = $"التاريخ: {invoice.InvoiceDate:yyyy/MM/dd} | الصافي: {invoice.FinalAmount:N2}";

                // دالة التصدير إلى Excel
                Action<string> excelExport = (filePath) =>
                {
                    var excelData = _reportService.GenerateExcel(invoice, settings);
                    System.IO.File.WriteAllBytes(filePath, excelData);
                };

                previewVm.LoadReport(pdfData, title, info, excelExport);
            }

            previewWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"حدث خطأ أثناء تجهيز الفاتورة للطباعة:\n{ex.Message}");
        }
    }

    private void NavigateToEditor(object? parameter)
    {
        // إذا كان كائن InvoiceDto نمرره كما هو، وإذا كان null نمرر الـ TypeId
        object param = parameter ?? InvoiceTypeId;
        _navigationService.NavigateTo<InvoiceEditorViewModel>(param);
    }

    private void NavigateToDetails(InvoiceDto? invoice)
    {
        if (invoice == null) return;
        _navigationService.NavigateTo<InvoiceDetailsViewModel>(invoice);
    }
}
