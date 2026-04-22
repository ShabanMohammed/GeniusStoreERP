using GeniusStoreERP.Application.Users.Commands.CreateUser;
using GeniusStoreERP.Application.Users.Commands.UpdateUser;
using GeniusStoreERP.Application.Users.Queries.GetUserById;
using GeniusStoreERP.Application.Users.Commands.SeedRoles;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Users;

public class UserManagementEditorViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;

    private string _id = string.Empty;
    private string _userName = string.Empty;
    private string _fullName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _role = "Sales";
    private bool _isEditMode;

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public ObservableCollection<string> Roles { get; } = new() { "Admin", "Sales", "Warehouse", "Accountant" };

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public UserManagementEditorViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;

        SaveCommand = new AsyncRelayCommand((_, _) => SaveAsync());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<UserManagementListViewModel>());
    }

    public override void Initialize(object? parameter)
    {
        if (parameter is string id)
        {
            _id = id;
            IsEditMode = true;
            _ = LoadUserAsync();
        }
        else
        {
            IsEditMode = false;
            // Ensure roles are seeded when opening editor for new user if needed
            // Actually, seeding should happen at startup.
        }
    }

    private async Task LoadUserAsync()
    {
        try
        {
            var user = await _mediator.Send(new GetUserByIdQuery(_id));
            UserName = user.UserName;
            FullName = user.FullName;
            Email = user.Email;
            Role = user.Role;
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
            _navigationService.NavigateTo<UserManagementListViewModel>();
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            bool success;
            if (IsEditMode)
            {
                var command = new UpdateUserCommand(_id, FullName, Email, Role);
                success = await _mediator.Send(command);
            }
            else
            {
                var command = new CreateUserCommand(UserName, Password, FullName, Email, Role);
                success = await _mediator.Send(command);
            }

            if (success)
            {
                _navigationService.NavigateTo<UserManagementListViewModel>();
            }
            else
            {
                MessageBoxService.ShowError("فشلت عملية الحفظ. يرجى التأكد من البيانات المدخلة.");
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }
}
