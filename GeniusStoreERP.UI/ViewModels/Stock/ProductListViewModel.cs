using GeniusStoreERP.Application.Products.Queries.GetProductById;
using GeniusStoreERP.Application.Stock.Products.Commands.DeleteProduct;
using GeniusStoreERP.Application.Stock.Products.Queries.GetProducts;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Stock;

public class ProductListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private string _searchText = string.Empty;
    private int _pageSize = 10;
    private int _currentPage = 1;
    private int _totalItems = 0;
    private ProductDto? _selectedProduct;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                CurrentPage = 1;
                _ = LoadProductsAsync();
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
                _ = LoadProductsAsync();
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
                _ = LoadProductsAsync();
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

    public ProductDto? SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
    }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public ObservableCollection<ProductDto> Products { get; } = new();

    public ICommand SearchCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ViewTransactionsCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand IncreasePageSizeCommand { get; }
    public ICommand DecreasePageSizeCommand { get; }

    public ProductListViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;
        SearchCommand = new AsyncRelayCommand((_, _) => LoadProductsAsync());
        AddCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProductEditViewModel>());
        EditCommand = new RelayCommand(p =>
        {
            var product = p as ProductDto ?? SelectedProduct;
            if (product != null)
            {
                _navigationService.NavigateTo<ProductEditViewModel>(product);
            }
        });
        ViewTransactionsCommand = new RelayCommand(p =>
        {
            var product = p as ProductDto ?? SelectedProduct;
            if (product != null)
            {
                _navigationService.NavigateTo<ProductTransactionsViewModel>(product);
            }
        });
        DeleteCommand = new AsyncRelayCommand((p, _) => DeleteProduct(p as ProductDto));
        NextPageCommand = new AsyncRelayCommand(
            (_, _) =>
            {
                if (CurrentPage < TotalPages)
                    CurrentPage++;
                return Task.CompletedTask;
            },
            _ => CurrentPage < TotalPages
        );
        PreviousPageCommand = new AsyncRelayCommand(
            (_, _) =>
            {
                if (CurrentPage > 1)
                    CurrentPage--;
                return Task.CompletedTask;
            },
            _ => CurrentPage > 1
        );
        FirstPageCommand = new AsyncRelayCommand(
            (_, _) =>
            {
                CurrentPage = 1;
                return Task.CompletedTask;
            },
            _ => CurrentPage > 1
        );
        LastPageCommand = new AsyncRelayCommand(
            (_, _) =>
            {
                if (TotalPages > 0)
                    CurrentPage = TotalPages;
                return Task.CompletedTask;
            },
            _ => CurrentPage < TotalPages
        );

        IncreasePageSizeCommand = new RelayCommand(_ => PageSize += 1);
        DecreasePageSizeCommand = new RelayCommand(_ =>
        {
            if (PageSize > 1)
                PageSize -= 1;
        });

        _ = LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var query = new GetProductsQuery(SearchText, null, CurrentPage, PageSize);
            var result = await _mediator.Send(query);

            Products.Clear();
            if (result?.Items != null)
            {
                foreach (var product in result.Items)
                {
                    Products.Add(product);
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

    private async Task DeleteProduct(ProductDto? product)
    {
        if (product == null)
            return;
        var result = MessageBoxService.ShowConfirmation(
            $"هل أنت متأكد أنك تريد حذف المنتج '{product.Name}'؟"
        );
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            var command = new DeleteProductCommand(product.Id);
            try
            {
                await _mediator.Send(command);
                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError(ex.Message);
            }
        }
    }
}
