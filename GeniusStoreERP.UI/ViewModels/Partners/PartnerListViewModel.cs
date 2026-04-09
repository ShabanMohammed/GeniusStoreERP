using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Application.Partners.Commands.DeletePartner;
using GeniusStoreERP.Application.Partners.Commands.UpdatePartner;
using GeniusStoreERP.Application.Partners.Queries.GetPartners;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Partners;

public class PartnerListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private string _searchText = string.Empty;
    private int _pageSize = 10;
    private int _currentPage = 1;
    private int _totalItems = 0;
    private PartnerDto? _selectedPartner;

    private bool _isSupplierFilter;
    private bool _isCustomerFilter;

    public bool IsSupplierFilter
    {
        get => _isSupplierFilter;
        set
        {
            if (SetProperty(ref _isSupplierFilter, value))
            {
                CurrentPage = 1;
                _ = LoadPartnersAsync();
            }
        }
    }

    public bool IsCustomerFilter
    {
        get => _isCustomerFilter;
        set
        {
            if (SetProperty(ref _isCustomerFilter, value))
            {
                CurrentPage = 1;
                _ = LoadPartnersAsync();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                CurrentPage = 1;
                _ = LoadPartnersAsync();
            }
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value))
            {
                OnPropertyChanged(nameof(TotalPages));
                CurrentPage = 1;
                _ = LoadPartnersAsync();
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
                _ = LoadPartnersAsync();
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

    public PartnerDto? SelectedPartner
    {
        get => _selectedPartner;
        set => SetProperty(ref _selectedPartner, value);
    }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public ObservableCollection<PartnerDto> Partners { get; } = new();

    public string ModeTitle => IsSupplierFilter && IsCustomerFilter ? "إدارة الشركاء" 
                               : IsSupplierFilter ? "إدارة الموردين" 
                               : "إدارة العملاء";

    public string AddButtonTitle => IsSupplierFilter && IsCustomerFilter ? "إضافة شريك" 
                               : IsSupplierFilter ? "إضافة مورد" 
                               : "إضافة عميل";

    public ICommand SearchCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand UpgradeRoleCommand { get; }
    public ICommand ToggleRoleCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand IncreasePageSizeCommand { get; }
    public ICommand DecreasePageSizeCommand { get; }
    public ICommand ViewStatementCommand { get; }

    public PartnerListViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;
        ViewStatementCommand = new RelayCommand(p =>
        {
            var partner = p as PartnerDto ?? SelectedPartner;
            if (partner != null)
            {
                _navigationService.NavigateTo<PartnerStatementViewModel>(partner.Id);
            }
        });
        SearchCommand = new AsyncRelayCommand((_, _) => LoadPartnersAsync());
        AddCommand = new RelayCommand(_ => {
            var partnerDto = new PartnerDto(0, "", "", "", "", IsSupplierFilter, IsCustomerFilter);
            _navigationService.NavigateTo<PartnerEditViewModel>(partnerDto);
        });
        EditCommand = new RelayCommand(p =>
        {
            var partner = p as PartnerDto ?? SelectedPartner;
            if (partner != null)
            {
                _navigationService.NavigateTo<PartnerEditViewModel>(partner);
            }
        });
        DeleteCommand = new AsyncRelayCommand((p, _) => DeletePartner(p as PartnerDto));
        UpgradeRoleCommand = new AsyncRelayCommand((p, _) => UpgradePartnerRole(p as PartnerDto));
        ToggleRoleCommand = new AsyncRelayCommand((p, _) => TogglePartnerRoleAsync(p as PartnerDto));
        
        NextPageCommand = new AsyncRelayCommand((_, _) => { if (CurrentPage < TotalPages) CurrentPage++; return Task.CompletedTask; }, _ => CurrentPage < TotalPages);
        PreviousPageCommand = new AsyncRelayCommand((_, _) => { if (CurrentPage > 1) CurrentPage--; return Task.CompletedTask; }, _ => CurrentPage > 1);
        FirstPageCommand = new AsyncRelayCommand((_, _) => { CurrentPage = 1; return Task.CompletedTask; }, _ => CurrentPage > 1);
        LastPageCommand = new AsyncRelayCommand((_, _) => { if (TotalPages > 0) CurrentPage = TotalPages; return Task.CompletedTask; }, _ => CurrentPage < TotalPages);

        IncreasePageSizeCommand = new RelayCommand(_ => PageSize += 1);
        DecreasePageSizeCommand = new RelayCommand(_ => { if (PageSize > 1) PageSize -= 1; });

        // نؤجل التحميل حتى الاستدعاء من التنقل
    }

    public override void Initialize(object? parameter)
    {
        if (parameter is string mode)
        {
            if (mode == "Suppliers")
            {
                _isSupplierFilter = true;
                _isCustomerFilter = false;
            }
            else if (mode == "Customers")
            {
                _isSupplierFilter = false;
                _isCustomerFilter = true;
            }
            else
            {
                _isSupplierFilter = true;
                _isCustomerFilter = true;
            }
            
            OnPropertyChanged(nameof(IsSupplierFilter));
            OnPropertyChanged(nameof(IsCustomerFilter));
            OnPropertyChanged(nameof(ModeTitle));
            OnPropertyChanged(nameof(AddButtonTitle));
        }
        
        _ = LoadPartnersAsync();
    }

    private async Task UpgradePartnerRole(PartnerDto? partner)
    {
        if (partner == null) return;
        
        string targetRole = partner.IsCustomer ? "مورد" : "عميل";
        var result = MessageBoxService.ShowConfirmation($"هل تريد إضافة صفة '{targetRole}' لهذا الشريك ليصبح (عميل ومورد) في نفس الوقت؟");
        
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                var command = new GeniusStoreERP.Application.Partners.Commands.UpgradePartnerRole.UpgradePartnerRoleCommand(partner.Id);
                await _mediator.Send(command);
                await LoadPartnersAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError(ex.Message);
            }
        }
    }


    private async Task TogglePartnerRoleAsync(PartnerDto? partner)
    {
        if (partner == null) return;

        bool newIsCustomer = partner.IsCustomer;
        bool newIsSupplier = partner.IsSupplier;
        string action = "";

        if (IsCustomerFilter) // نحن في شاشة العملاء، نريد تبديل حالة المورد
        {
            newIsSupplier = !partner.IsSupplier;
            action = newIsSupplier ? "إضافة كـ مورد" : "إلغاء صفة مورد";
        }
        else if (IsSupplierFilter) // نحن في شاشة الموردين، نريد تبديل حالة العميل
        {
            newIsCustomer = !partner.IsCustomer;
            action = newIsCustomer ? "إضافة كـ عميل" : "إلغاء صفة عميل";
        }

        if (string.IsNullOrEmpty(action)) return;

        if (!newIsCustomer && !newIsSupplier)
        {
            MessageBoxService.ShowError("لا يمكن إلغاء الصفتين معاً، يجب أن يكون الشريك إما عميلاً أو مورداً على الأقل.");
            return;
        }

        var result = MessageBoxService.ShowConfirmation($"هل أنت متأكد أنك تريد {action} لهذا الشريك؟");
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                var command = new UpdatePartnerCommand(partner.Id, partner.Name, partner.Email, partner.PhoneNumber, partner.Address, newIsCustomer, newIsSupplier);
                await _mediator.Send(command);
                await LoadPartnersAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError(ex.Message);
            }
        }
    }

    private async Task LoadPartnersAsync()
    {
        try
        {
            var command = new GetPartnersCommand(SearchText, IsSupplierFilter, IsCustomerFilter, PageSize, CurrentPage);
            var result = await _mediator.Send(command);

            Partners.Clear();
            if (result?.Items != null)
            {
                foreach (var partner in result.Items)
                {
                    Partners.Add(partner);
                }
                TotalItems = result.TotalCount;
                if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }

    private async Task DeletePartner(PartnerDto? partner)
    {
        if (partner == null) return;

        // 1. التأكيد المبدئي للحذف
        var confirmResult = MessageBoxService.ShowConfirmation($"هل أنت متأكد أنك تريد حذف الشريك '{partner.Name}'؟");
        if (confirmResult != System.Windows.MessageBoxResult.Yes) return;

        bool deleteEntirely = true;

        // 2. التحقق مما إذا كان يمتلك الدورين
        if (partner.IsCustomer && partner.IsSupplier && !(IsCustomerFilter && IsSupplierFilter))
        {
            string currentRole = IsCustomerFilter ? "عميل" : "مورد";
            string otherRole = IsCustomerFilter ? "مورد" : "عميل";

            var scopeResult = MessageBoxService.ShowConfirmation(
                $"هذا الشريك مسجل كـ (عميل ومورد) معاً.{Environment.NewLine}{Environment.NewLine}" +
                $"هل تريد حذفه من كلا الدورين نهائياً (نعم)؟{Environment.NewLine}" +
                $"أم حذف صفة الـ {currentRole} فقط والإبقاء عليه كـ {otherRole} (لا)؟",
                "تحديد نطاق الحذف");

            if (scopeResult == System.Windows.MessageBoxResult.No)
            {
                deleteEntirely = false;
            }
            else if (scopeResult != System.Windows.MessageBoxResult.Yes)
            {
                return; // إلغاء العملية بالكامل
            }
        }

        try
        {
            if (deleteEntirely)
            {
                var command = new DeletePartnerCommand(partner.Id);
                await _mediator.Send(command);
            }
            else
            {
                // حذف الدور الحالي فقط عبر تحديث الصلاحيات
                bool newIsCustomer = IsCustomerFilter ? false : partner.IsCustomer;
                bool newIsSupplier = IsSupplierFilter ? false : partner.IsSupplier;

                var command = new UpdatePartnerCommand(partner.Id, partner.Name, partner.Email, partner.PhoneNumber, partner.Address, newIsCustomer, newIsSupplier);
                await _mediator.Send(command);
            }

            await LoadPartnersAsync();
        }
        catch (NotFoundException)
        {
            MessageBoxService.ShowError("هذا الشريك غير موجود");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }
}
