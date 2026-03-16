using GeniusStoreERP.UI.Common;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace GeniusStoreERP.UI.ViewModels;

public class MainViewModel : BaseViewModel
{
    private string _fullName = string.Empty;
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    private string _userRole = string.Empty;
    public string UserRole
    {
        get => _userRole;
        set => SetProperty(ref _userRole, value);
    }

    public string CurrentDate => DateTime.Now.ToString("dd/MM/yyyy");
    public string CurrentTime => DateTime.Now.ToString("HH:mm");

    private NavItem? _selectedNavItem;
    public NavItem? SelectedNavItem
    {
        get => _selectedNavItem;
        set
        {
            if (SetProperty(ref _selectedNavItem, value))
            {
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }
    }

    public object? CurrentViewModel => SelectedNavItem?.TargetViewModel;

    public ObservableCollection<NavItem> NavItems { get; } = new();

    public ICommand SelectNavItemCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel()
    {
        LogoutCommand = new RelayCommand(_ => 
        {
            var loginView = App.ServiceProvider.GetRequiredService<Views.LoginView>();
            loginView.Show();
            
            var currentWindow = System.Windows.Application.Current.MainWindow;
            System.Windows.Application.Current.MainWindow = loginView;
            
            currentWindow?.Close();
        });

        SelectNavItemCommand = new RelayCommand(p => 
        {
            if (p is NavItem item)
            {
                foreach (var i in NavItems) i.IsSelected = false;
                item.IsSelected = true;
                SelectedNavItem = item;
            }
        });

        InitializeNavItems();
    }

    private void InitializeNavItems()
    {
        var dashboardVm = App.ServiceProvider.GetRequiredService<DashboardViewModel>();

        NavItems.Add(new NavItem 
        { 
            Title = "الرئيسية", 
            IconKey = "IconHome", 
            IsSelected = true,
            TargetViewModel = dashboardVm
        });
        
        NavItems.Add(new NavItem { Title = "المبيعات", IconKey = "IconSuccess" });
        NavItems.Add(new NavItem { Title = "المخازن", IconKey = "IconInformation" });
        NavItems.Add(new NavItem { Title = "المشتريات", IconKey = "IconSettings" });
        NavItems.Add(new NavItem { Title = "العملاء", IconKey = "IconUsers" });
        NavItems.Add(new NavItem { Title = "الإعدادات", IconKey = "IconSettings" });

        SelectedNavItem = NavItems[0];
    }
}
