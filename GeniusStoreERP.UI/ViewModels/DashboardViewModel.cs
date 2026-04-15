using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;

namespace GeniusStoreERP.UI.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;

    private string _companyName = "Genius Store ERP";
    public string CompanyName
    {
        get => _companyName;
        set => SetProperty(ref _companyName, value);
    }

    private byte[]? _companyLogo;
    public byte[]? CompanyLogo
    {
        get => _companyLogo;
        set => SetProperty(ref _companyLogo, value);
    }

    public ICommand NavigateToSalesCommand { get; }
    public ICommand NavigateToPurchasesCommand { get; }
    public ICommand NavigateToInventoryCommand { get; }
    public ICommand NavigateToReportsCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }

    public DashboardViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;

        NavigateToSalesCommand = new RelayCommand(_ => _navigationService.NavigateTo<Transactions.InvoiceListViewModel>(1));
        NavigateToPurchasesCommand = new RelayCommand(_ => _navigationService.NavigateTo<Transactions.InvoiceListViewModel>(2));
        NavigateToInventoryCommand = new RelayCommand(_ => _navigationService.NavigateTo<Stock.ProductListViewModel>());
        NavigateToReportsCommand = new RelayCommand(_ => _navigationService.NavigateTo<ReportsMainViewModel>());
        NavigateToSettingsCommand = new RelayCommand(_ => _navigationService.NavigateTo<GeneralSettingEditViewModel>());

        LoadCompanySettings();
    }

    public override void Initialize(object? parameter)
    {
        LoadCompanySettings();
    }

    private async void LoadCompanySettings()
    {
        try
        {
            var result = await _mediator.Send(new GetGeneralSettingsQuery());
            if (result != null)
            {
                CompanyName = result.CompanyName;
                CompanyLogo = result.Logo;
            }
        }
        catch (Exception)
        {
            // Fallback for company name if error occurs
            CompanyName = "الرئيسية";
        }
    }
}

