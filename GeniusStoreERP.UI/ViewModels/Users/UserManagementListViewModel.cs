using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Users.Commands.DeleteUser;
using GeniusStoreERP.Application.Users.Queries.GetUsers;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Users;

public class UserManagementListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private string _searchText = string.Empty;
    private int _pageSize = 10;
    private int _currentPage = 1;
    private int _totalItems = 0;
    private UserDto? _selectedUser;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                CurrentPage = 1;
                _ = LoadUsersAsync();
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
                CurrentPage = 1;
                _ = LoadUsersAsync();
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
                _ = LoadUsersAsync();
            }
        }
    }

    public int TotalItems
    {
        get => _totalItems;
        set => SetProperty(ref _totalItems, value);
    }

    public UserDto? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public ObservableCollection<UserDto> Users { get; } = new();

    public ICommand SearchCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    public UserManagementListViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;

        SearchCommand = new AsyncRelayCommand((_, _) => LoadUsersAsync());
        AddCommand = new RelayCommand(_ => _navigationService.NavigateTo<UserManagementEditorViewModel>());
        EditCommand = new RelayCommand(p =>
        {
            var user = p as UserDto ?? SelectedUser;
            if (user != null)
            {
                _navigationService.NavigateTo<UserManagementEditorViewModel>(user.Id);
            }
        });
        DeleteCommand = new AsyncRelayCommand((p, _) => DeleteUserAsync(p as UserDto));

        NextPageCommand = new AsyncRelayCommand((_, _) => { if (CurrentPage < TotalPages) CurrentPage++; return Task.CompletedTask; });
        PreviousPageCommand = new AsyncRelayCommand((_, _) => { if (CurrentPage > 1) CurrentPage--; return Task.CompletedTask; });
    }

    public override void Initialize(object? parameter)
    {
        _ = LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            var query = new GetUsersQuery(SearchText, PageSize, CurrentPage);
            var result = await _mediator.Send(query);

            Users.Clear();
            foreach (var user in result.Items)
            {
                Users.Add(user);
            }
            TotalItems = result.TotalCount;
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }

    private async Task DeleteUserAsync(UserDto? user)
    {
        if (user == null) return;

        if (MessageBoxService.ShowConfirmation($"هل أنت متأكد من حذف المستخدم '{user.FullName}'؟") == System.Windows.MessageBoxResult.Yes)
        {
            try
            {
                var command = new DeleteUserCommand(user.Id);
                await _mediator.Send(command);
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError(ex.Message);
            }
        }
    }
}
