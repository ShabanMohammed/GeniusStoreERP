using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Dtos.ListItemDto;
using GeniusStoreERP.Application.Finances.Commands;
using GeniusStoreERP.Application.Finances.Queries;
using GeniusStoreERP.Application.Partners.Queries.GetPartnerItems;
using GeniusStoreERP.Domain.Entities.Finances;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;
using GeniusStoreERP.Application.Common.Interfaces;

namespace GeniusStoreERP.UI.ViewModels.Finances;

public class TreasuryViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly INavigationService _navigationService;
    private readonly IFinanceReportService _reportService;

    private ObservableCollection<TreasuryDto> _treasuries = new();
    private TreasuryDto? _selectedTreasury;
    private ObservableCollection<TreasuryTransactionDto> _transactions = new();
    private string _pageTitle = "إدارة الخزينة";

    public string PageTitle { get => _pageTitle; set => SetProperty(ref _pageTitle, value); }
    public ObservableCollection<TreasuryDto> Treasuries { get => _treasuries; set => SetProperty(ref _treasuries, value); }
    public TreasuryDto? SelectedTreasury
    {
        get => _selectedTreasury;
        set
        {
            if (SetProperty(ref _selectedTreasury, value))
            {
                _ = LoadTransactions();
            }
        }
    }
    public ObservableCollection<TreasuryTransactionDto> Transactions { get => _transactions; set => SetProperty(ref _transactions, value); }

    public ICommand LoadDataCommand { get; }
    public ICommand AddTreasuryCommand { get; }
    public ICommand AddTransactionCommand { get; }
    public ICommand PrintReceiptCommand { get; }

    public TreasuryViewModel(IMediator mediator, INavigationService navigationService, IFinanceReportService reportService)
    {
        _mediator = mediator;
        _navigationService = navigationService;
        _reportService = reportService;

        LoadDataCommand = new AsyncRelayCommand(async (p, c) => await LoadData());
        AddTreasuryCommand = new AsyncRelayCommand(async (p, c) => await ShowAddTreasuryDialog());
        AddTransactionCommand = new AsyncRelayCommand(async (p, c) => await ShowAddTransactionDialog());
        PrintReceiptCommand = new AsyncRelayCommand(async (p, c) => await PrintReceipt((TreasuryTransactionDto)p));
    }

    public override async void Initialize(object? parameter)
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            var result = await _mediator.Send(new GetTreasuriesQuery());
            Treasuries = new ObservableCollection<TreasuryDto>(result);
            if (SelectedTreasury == null && Treasuries.Any())
            {
                SelectedTreasury = Treasuries.First();
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في تحميل الخزائن: {ex.Message}");
        }
    }

    private async Task LoadTransactions()
    {
        if (SelectedTreasury == null) return;

        try
        {
            var result = await _mediator.Send(new GetTreasuryTransactionsQuery(SelectedTreasury.Id));
            Transactions = new ObservableCollection<TreasuryTransactionDto>(result);
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في تحميل العمليات: {ex.Message}");
        }
    }

    private async Task ShowAddTreasuryDialog()
    {
        _navigationService.NavigateTo<AddTreasuryViewModel>();
    }

    private async Task ShowAddTransactionDialog()
    {
        if (SelectedTreasury == null)
        {
            MessageBoxService.ShowWarning("يرجى اختيار خزينة أولاً");
            return;
        }

        // Logic to navigate to or show the Transaction creation window
        // I'll create a dedicated window for this.
        _navigationService.NavigateTo<TreasuryTransactionViewModel>(SelectedTreasury);
    }

    private async Task PrintReceipt(TreasuryTransactionDto transaction)
    {
        if (transaction == null) return;

        try
        {
            var settings = await _mediator.Send(new GetGeneralSettingsQuery());
            var pdfData = _reportService.GenerateTreasuryReceiptPdf(transaction, settings);

            // Navigate to Preview window
            _navigationService.ShowWindow<ReportPreviewViewModel>(pdfData, "معاينة إيصال الخزينة");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ أثناء طباعة الإيصال: {ex.Message}");
        }
    }
}
