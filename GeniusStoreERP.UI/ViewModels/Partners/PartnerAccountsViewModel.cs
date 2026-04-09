using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Partners.Queries.GetPartnerAccounts;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;

namespace GeniusStoreERP.UI.ViewModels.Partners
{
    public class PartnerAccountsViewModel : BaseViewModel
    {
        private readonly IMediator _mediator;
        private readonly INavigationService _navigationService;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = LoadAccountsAsync();
                }
            }
        }

        private bool _isCustomerFilter = true;
        public bool IsCustomerFilter
        {
            get => _isCustomerFilter;
            set
            {
                if (SetProperty(ref _isCustomerFilter, value))
                {
                    _ = LoadAccountsAsync();
                }
            }
        }

        private bool _isSupplierFilter = true;
        public bool IsSupplierFilter
        {
            get => _isSupplierFilter;
            set
            {
                if (SetProperty(ref _isSupplierFilter, value))
                {
                    _ = LoadAccountsAsync();
                }
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private int _pageSize = 15;
        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ObservableCollection<PartnerAccountDto> Accounts { get; } = new();

        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewStatementCommand { get; }

        public PartnerAccountsViewModel(IMediator mediator, INavigationService navigationService)
        {
            _mediator = mediator;
            _navigationService = navigationService;

            NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPages) { CurrentPage++; _ = LoadAccountsAsync(); } });
            PreviousPageCommand = new RelayCommand(_ => { if (CurrentPage > 1) { CurrentPage--; _ = LoadAccountsAsync(); } });
            RefreshCommand = new RelayCommand(_ => _ = LoadAccountsAsync());
            ViewStatementCommand = new RelayCommand(p => OnViewStatement(p));

            _ = LoadAccountsAsync();
        }

        public override async void Initialize(object? parameter)
        {
            await LoadAccountsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            try
            {
                var query = new GetPartnerAccountsQuery(
                    SearchText,
                    IsSupplierFilter,
                    IsCustomerFilter,
                    CurrentPage,
                    PageSize
                );

                var result = await _mediator.Send(query);

                Accounts.Clear();
                foreach (var item in result.Items)
                {
                    Accounts.Add(item);
                }

                TotalItems = result.TotalCount;
                OnPropertyChanged(nameof(TotalPages));
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"خطأ في موازين الحسابات: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnViewStatement(object? parameter)
        {
            if (parameter is PartnerAccountDto account)
            {
                _navigationService.NavigateTo<PartnerStatementViewModel>(account.Id);
            }
        }
    }
}
