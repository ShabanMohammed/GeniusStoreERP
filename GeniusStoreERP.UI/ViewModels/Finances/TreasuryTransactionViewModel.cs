using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Dtos.ListItemDto;
using GeniusStoreERP.Application.Finances.Commands;
using GeniusStoreERP.Application.Partners.Queries.GetPartnerItems;
using GeniusStoreERP.Domain.Entities.Finances;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Finances;

public class TreasuryTransactionViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly INavigationService _navigationService;

    private int _treasuryId;
    private string _treasuryName = string.Empty;
    private decimal _amount;
    private DateTime _transactionDate = DateTime.Now;
    private TreasuryTransactionType _type = TreasuryTransactionType.CashIn;
    private PartnerListItemDto? _selectedPartner;
    private ObservableCollection<PartnerListItemDto> _partners = new();
    private string? _notes;
    private string? _referenceNumber;

    public int TreasuryId { get => _treasuryId; set => SetProperty(ref _treasuryId, value); }
    public string TreasuryName { get => _treasuryName; set => SetProperty(ref _treasuryName, value); }
    public decimal Amount { get => _amount; set => SetProperty(ref _amount, value); }
    public DateTime TransactionDate { get => _transactionDate; set => SetProperty(ref _transactionDate, value); }
    public TreasuryTransactionType Type { get => _type; set => SetProperty(ref _type, value); }
    public PartnerListItemDto? SelectedPartner { get => _selectedPartner; set => SetProperty(ref _selectedPartner, value); }
    public ObservableCollection<PartnerListItemDto> Partners { get => _partners; set => SetProperty(ref _partners, value); }
    public string? Notes { get => _notes; set => SetProperty(ref _notes, value); }
    public string? ReferenceNumber { get => _referenceNumber; set => SetProperty(ref _referenceNumber, value); }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public TreasuryTransactionViewModel(IMediator mediator, INavigationService navigationService)
    {
        _mediator = mediator;
        _navigationService = navigationService;

        SaveCommand = new AsyncRelayCommand(async (p, c) => await SaveTransaction());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<TreasuryViewModel>());
    }

    public override async void Initialize(object? parameter)
    {
        if (parameter is TreasuryDto treasury)
        {
            TreasuryId = treasury.Id;
            TreasuryName = treasury.Name;
        }

        await LoadPartners();
    }

    private async Task LoadPartners()
    {
        try
        {
            // Load Customers, Suppliers, and Shareholders
            var result = await _mediator.Send(new GetPartnerItemsCommand(IsCustomer: true, IsSupplier: true, IsShareholder: true));
            Partners = new ObservableCollection<PartnerListItemDto>(result);
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في تحميل الشركاء: {ex.Message}");
        }
    }

    private async Task SaveTransaction()
    {
        if (Amount <= 0)
        {
            MessageBoxService.ShowWarning("يجب أن يكون المبلغ أكبر من صفر");
            return;
        }

        try
        {
            var command = new CreateTreasuryTransactionCommand(
                TreasuryId,
                Amount,
                TransactionDate,
                Type,
                SelectedPartner?.Id,
                null,
                ReferenceNumber,
                Notes
            );

            await _mediator.Send(command);
            MessageBoxService.ShowSuccess("تم حفظ العملية بنجاح وتحديث حساب الخزينة والشريك");
            _navigationService.NavigateTo<TreasuryViewModel>();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }
}
