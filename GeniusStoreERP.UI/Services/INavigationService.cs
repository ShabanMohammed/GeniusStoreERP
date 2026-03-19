using GeniusStoreERP.UI.Common;

namespace GeniusStoreERP.UI.Services;

public interface INavigationService
{
    event Action<BaseViewModel>? Navigated;

    TViewModel NavigateTo<TViewModel>() where TViewModel : BaseViewModel;
    TViewModel NavigateTo<TViewModel>(object? parameter) where TViewModel : BaseViewModel;
    TViewModel ShowWindow<TViewModel>(object? parameter, string title = "") where TViewModel : BaseViewModel;
    BaseViewModel? CurrentViewModel { get; }
}
