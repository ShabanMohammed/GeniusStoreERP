using GeniusStoreERP.UI.Common;

namespace GeniusStoreERP.UI.ViewModels;

public class NavItem : BaseViewModel
{
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _iconKey = string.Empty;
    public string IconKey
    {
        get => _iconKey;
        set => SetProperty(ref _iconKey, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public object? TargetViewModel { get; set; }
}
