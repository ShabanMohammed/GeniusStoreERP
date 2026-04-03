using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using GeniusStoreERP.Application.Stock.Products.Queries.GetProductTransactions;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace GeniusStoreERP.UI.ViewModels.Stock;

public class ProductTransactionsViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;
    private ProductDto? _product;
    private int _pageSize = 10;
    private int _currentPage = 1;
    private int _totalItems = 0;
    private DateTime? _startDate;
    private DateTime? _endDate;
    private string _title = "حركة الصنف";

    public ProductDto? Product
    {
        get => _product;
        set
        {
            SetProperty(ref _product, value);
            Title = $"حركة الصنف: {value?.Name}";
        }
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public DateTime? StartDate
    {
        get => _startDate;
        set
        {
            if (SetProperty(ref _startDate, value))
            {
                CurrentPage = 1;
                _ = LoadTransactionsAsync();
            }
        }
    }

    public DateTime? EndDate
    {
        get => _endDate;
        set
        {
            if (SetProperty(ref _endDate, value))
            {
                CurrentPage = 1;
                _ = LoadTransactionsAsync();
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
                _ = LoadTransactionsAsync();
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
                _ = LoadTransactionsAsync();
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

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public ObservableCollection<ProductTransactionDto> Transactions { get; } = new();

    public ICommand BackCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand IncreasePageSizeCommand { get; }
    public ICommand DecreasePageSizeCommand { get; }
    public ICommand ClearDatesCommand { get; }

    public ProductTransactionsViewModel(INavigationService navigationService, IMediator mediator, IServiceProvider serviceProvider)
    {
        _navigationService = navigationService;
        _mediator = mediator;
        _serviceProvider = serviceProvider;
        
        BackCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProductListViewModel>());
        
        NextPageCommand = new AsyncRelayCommand(
            (_, _) => { if (CurrentPage < TotalPages) CurrentPage++; return Task.CompletedTask; },
            _ => CurrentPage < TotalPages
        );
        PreviousPageCommand = new AsyncRelayCommand(
            (_, _) => { if (CurrentPage > 1) CurrentPage--; return Task.CompletedTask; },
            _ => CurrentPage > 1
        );
        FirstPageCommand = new AsyncRelayCommand(
            (_, _) => { CurrentPage = 1; return Task.CompletedTask; },
            _ => CurrentPage > 1
        );
        LastPageCommand = new AsyncRelayCommand(
            (_, _) => { if (TotalPages > 0) CurrentPage = TotalPages; return Task.CompletedTask; },
            _ => CurrentPage < TotalPages
        );

        IncreasePageSizeCommand = new RelayCommand(_ => PageSize += 1);
        DecreasePageSizeCommand = new RelayCommand(_ => { if (PageSize > 1) PageSize -= 1; });
        
        ClearDatesCommand = new RelayCommand(_ => {
            _startDate = null;
            _endDate = null;
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
            CurrentPage = 1;
            _ = LoadTransactionsAsync();
        });
    }

    public override void Initialize(object? parameter)
    {
        if (parameter is ProductDto product)
        {
            Product = product;
            _ = LoadTransactionsAsync();
        }
    }

    private async Task LoadTransactionsAsync()
    {
        if (Product == null) return;
        
        try
        {
            var query = new GetProductTransactionsQuery(Product.Id, CurrentPage, PageSize, StartDate, EndDate);
            var result = await _mediator.Send(query);

            Transactions.Clear();
            if (result?.Items != null)
            {
                foreach (var transaction in result.Items)
                {
                    Transactions.Add(transaction);
                }
                TotalItems = result.TotalCount;
                if (CurrentPage > TotalPages && TotalPages > 0)
                    CurrentPage = TotalPages;
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }
}
