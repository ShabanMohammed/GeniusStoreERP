using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Transactions.Queries.GetInvoicesByType;
using GeniusStoreERP.Domain.Entities.Transactions;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Transactions;

public class InvoiceListViewModel : BaseViewModel
{

    private readonly IMediator _mediator;

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
    public ICommand EditInvoiceCommand { get; }
    public ICommand DeleteInvoiceCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand IncreasePageSizeCommand { get; }
    public ICommand DecreasePageSizeCommand { get; }

    public InvoiceListViewModel(IMediator mediator, INavigationService navigationService)
    {
        _mediator = mediator;
        _navigationService = navigationService;

        AddInvoiceCommand = new RelayCommand(_ => NavigateToEditor(null));
        EditInvoiceCommand = new RelayCommand(param => NavigateToEditor(param as InvoiceDto));
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
        // Logic for deletion if needed, or just a placeholder for now
    }

    private void NavigateToEditor(object? parameter)
    {
        // إذا كان كائن InvoiceDto نمرره كما هو، وإذا كان null نمرر الـ TypeId
        object param = parameter ?? InvoiceTypeId;
        _navigationService.NavigateTo<InvoiceEditorViewModel>(param);
    }
}
