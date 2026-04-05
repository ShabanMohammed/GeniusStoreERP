using GeniusStoreERP.Application.Finances.Commands;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Finances;

public class AddTreasuryViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly INavigationService _navigationService;

    private string _name = string.Empty;
    private string _code = string.Empty;
    private string? _description;

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public string Code { get => _code; set => SetProperty(ref _code, value); }
    public string? Description { get => _description; set => SetProperty(ref _description, value); }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public AddTreasuryViewModel(IMediator mediator, INavigationService navigationService)
    {
        _mediator = mediator;
        _navigationService = navigationService;

        SaveCommand = new AsyncRelayCommand(async (p, c) => await SaveTreasury());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<TreasuryViewModel>());
    }

    private async Task SaveTreasury()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            MessageBoxService.ShowWarning("يجب إدخال اسم الخزينة");
            return;
        }

        try
        {
            var command = new CreateTreasuryCommand(Name, Code, Description);
            await _mediator.Send(command);
            MessageBoxService.ShowSuccess("تم إضافة الخزينة بنجاح");
            _navigationService.NavigateTo<TreasuryViewModel>();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }
}
