using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.UI.ViewModels.Stock;
using GeniusStoreERP.UI.ViewModels.Partners;
using GeniusStoreERP.UI.ViewModels.Transactions;
using GeniusStoreERP.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GeniusStoreERP.UI.ViewModels.Finances;
using System.Windows;
using GeniusStoreERP.UI.ViewModels.Users;

namespace GeniusStoreERP.UI.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;

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
        set => SetProperty(ref _selectedNavItem, value);
    }

    public BaseViewModel? CurrentViewModel => _navigationService.CurrentViewModel;

    public ObservableCollection<NavItem> NavItems { get; } = new();

    public ICommand SelectNavItemCommand { get; }
    public ICommand LogoutCommand { get; }

    public MainViewModel()
        : this(App.ServiceProvider.GetRequiredService<INavigationService>())
    {
    }

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        _navigationService.Navigated += _ => OnPropertyChanged(nameof(CurrentViewModel));

        LogoutCommand = new RelayCommand(_ =>
        {
            if (MessageBoxService.ShowConfirmation("هل أنت متأكد من رغبتك في تسجيل الخروج؟", "تأكيد تسجيل الخروج") == System.Windows.MessageBoxResult.Yes)
            {
                // ✅ حل المشكلة الحقيقية: لا نستخدم نفس النسخة المنتهية الصلاحية
                // نقوم بإنشاء نسخة جديدة من نافذة تسجيل الدخول كل مرة
                var loginView = ActivatorUtilities.CreateInstance<LoginView>(App.ServiceProvider);

                // أولاً نحدد النافذة الجديدة كنافذة رئيسية قبل إغلاق القديمة
                System.Windows.Application.Current.MainWindow = loginView;
                
                // ثم نعرض النافذة الجديدة
                loginView.Show();
                
                // وأخيراً نغلق النافذة القديمة (النافذة الرئيسية)
                System.Windows.Application.Current.Windows.OfType<GeniusStoreERP.UI.Views.MainView>().FirstOrDefault()?.Close();
            }
        });

        SelectNavItemCommand = new RelayCommand(p =>
        {
            if (p is NavItem item)
            {
                // إذا كان للعنصر عناصر فرعية، نقوم بتبديل حالة التوسيع فقط
                if (item.SubItems.Any())
                {
                    item.IsExpanded = !item.IsExpanded;
                    return;
                }

                // إلغاء تحديد الكل (بما في ذلك العناصر الفرعية)
                foreach (var i in NavItems)
                {
                    i.IsSelected = false;
                    foreach (var sub in i.SubItems) sub.IsSelected = false;
                }

                item.IsSelected = true;
                SelectedNavItem = item;

                // التنقل بناءً على العنوان أو نوع ViewModel المستهدف
                switch (item.Title)
                {
                    case "الرئيسية":
                        _navigationService.NavigateTo<DashboardViewModel>();
                        break;
                    case "التصنيفات":
                        _navigationService.NavigateTo<CategoryListViewModel>();
                        break;
                    case "المنتجات":
                        _navigationService.NavigateTo<ProductListViewModel>();
                        break;
                    case "تسويات المخزون":
                        _navigationService.NavigateTo<StockAdjustmentListViewModel>();
                        break;
                    case "العملاء":
                        _navigationService.NavigateTo<PartnerListViewModel>("Customers");
                        break;
                    case "الموردين":
                        _navigationService.NavigateTo<PartnerListViewModel>("Suppliers");
                        break;
                    case "فواتير المبيعات":
                        _navigationService.NavigateTo<InvoiceListViewModel>(1);
                        break;
                    case "مرتجع المبيعات":
                        _navigationService.NavigateTo<InvoiceListViewModel>(3);
                        break;
                    case "فواتير المشتريات":
                        _navigationService.NavigateTo<InvoiceListViewModel>(2);
                        break;
                    case "مرتجع المشتريات":
                        _navigationService.NavigateTo<InvoiceListViewModel>(4);
                        break;
                    case "الإعدادات العامة":
                        _navigationService.NavigateTo<GeneralSettingEditViewModel>();
                        break;
                    case "الخزينة":
                        _navigationService.NavigateTo<TreasuryViewModel>();
                        break;
                    case "حسابات الشركاء":
                        _navigationService.NavigateTo<PartnerAccountsViewModel>();
                        break;
                    case "التقارير":
                        _navigationService.NavigateTo<ReportsMainViewModel>();
                        break;
                    case "إدارة المستخدمين":
                        _navigationService.NavigateTo<UserManagementListViewModel>();
                        break;
                }

            }
        });

        InitializeNavItems();
    }

    private void InitializeNavItems()
    {
        NavItems.Add(new NavItem
        {
            Title = "الرئيسية",
            IconKey = "IconHome",
            IsSelected = true,
            TargetViewModel = null
        });

        NavItems.Add(new NavItem
        {
            Title = "المخازن",
            IconKey = "IconInformation",
            SubItems =
            {
                new NavItem { Title = "التصنيفات", IconKey = "IconSettings" },
                new NavItem { Title = "المنتجات", IconKey = "IconInformation" },
                new NavItem { Title = "تسويات المخزون", IconKey = "IconExchange" }
            }
        });

        NavItems.Add(new NavItem
        {
            Title = "المبيعات",
            IconKey = "IconSuccess",
            SubItems =
            {
                new NavItem { Title = "فواتير المبيعات", IconKey = "IconInformation" },
                new NavItem { Title = "مرتجع المبيعات", IconKey = "IconExchange" }
            }
        });

        NavItems.Add(new NavItem
        {
            Title = "المشتريات",
            IconKey = "IconSettings",
            SubItems =
            {
                new NavItem { Title = "فواتير المشتريات", IconKey = "IconInformation" },
                new NavItem { Title = "مرتجع المشتريات", IconKey = "IconExchange" }
            }
        });
        NavItems.Add(new NavItem
        {
            Title = "الشركاء",
            IconKey = "IconUsers",
            SubItems =
            {
                new NavItem { Title = "العملاء", IconKey = "IconUsers" },
                new NavItem { Title = "الموردين", IconKey = "IconUsers" }
            }
        });
        NavItems.Add(new NavItem
        {
            Title = "المالية",
            IconKey = "IconSuccess",
            SubItems =
            {
                new NavItem { Title = "الخزينة", IconKey = "IconInformation" },
                new NavItem { Title = "حسابات الشركاء", IconKey = "IconInformation" }
            }
        });
        NavItems.Add(new NavItem
        {
            Title = "التقارير",
            IconKey = "IconActivity", // Update icon if needed
            TargetViewModel = null
        });

        NavItems.Add(new NavItem
        {
            Title = "الإعدادات",
            IconKey = "IconSettings",
            SubItems =
            {
                new NavItem { Title = "الإعدادات العامة", IconKey = "IconSettings" },
                new NavItem { Title = "إدارة المستخدمين", IconKey = "IconUsers" }
            }
        });

        SelectedNavItem = NavItems[0];

        // ضبط الشاشة الافتراضية عند بدء النظام
        _navigationService.NavigateTo<DashboardViewModel>();
    }
}
