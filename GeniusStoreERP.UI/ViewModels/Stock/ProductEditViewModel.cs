using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using GeniusStoreERP.Application.Stock.Categories.Queries.GetCategoriesList;
using GeniusStoreERP.Application.Stock.Products.Commands.CreeteProduct;
using GeniusStoreERP.Application.Stock.Products.Commands.UpdateProduct;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Stock;

public class ProductEditViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private int _id;
    private string _name = string.Empty;
    private string? _description;
    private int _categoryId;
    private decimal _price;
    private string? _sku;
    private string? _barcode;
    private decimal _stockQuantity;
    private decimal _reorderLevel;
    private string _title = "إضافة منتج جديد";

    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public int CategoryId
    {
        get => _categoryId;
        set => SetProperty(ref _categoryId, value);
    }

    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public string? SKU
    {
        get => _sku;
        set => SetProperty(ref _sku, value);
    }

    public string? Barcode
    {
        get => _barcode;
        set => SetProperty(ref _barcode, value);
    }

    public decimal StockQuantity
    {
        get => _stockQuantity;
        set => SetProperty(ref _stockQuantity, value);
    }

    public decimal ReorderLevel
    {
        get => _reorderLevel;
        set => SetProperty(ref _reorderLevel, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public ObservableCollection<CategoryListItemDto> Categories { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand GenerateBarcodeCommand { get; }

    public ProductEditViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;
        SaveCommand = new AsyncRelayCommand((_, _) => SaveAsync());
        CancelCommand = new RelayCommand(_ =>
            _navigationService.NavigateTo<ProductListViewModel>()
        );
        GenerateBarcodeCommand = new RelayCommand(_ => GenerateBarcode());
    }

    public override async void Initialize(object? parameter)
    {
        // Load categories for dropdown
        await LoadCategoriesAsync();

        if (parameter is ProductDto product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            CategoryId = product.CategoryId;
            Price = product.Price;
            SKU = product.SKU;
            Barcode = product.Barcode;
            StockQuantity = product.StockQuantity ?? 0;
            ReorderLevel = product.ReorderLevel ?? 0;
            Title = "تعديل المنتج";
        }
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var query = new GetCategoriesListQuery();
            var result = await _mediator.Send(query);

            Categories.Clear();
            if (result != null)
            {
                foreach (var category in result)
                {
                    Categories.Add(category);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في تحميل التصنيفات: {ex.Message}");
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (Id == 0)
            {
                var command = new CreateProductCommand(
                    Name,
                    Description,
                    Price,
                    StockQuantity,
                    ReorderLevel,
                    SKU,
                    Barcode,
                    CategoryId
                );
                await _mediator.Send(command);
            }
            else
            {
                var command = new UpdateProductCommand(
                    Id,
                    Name,
                    Description,
                    Price,
                    StockQuantity,
                    ReorderLevel,
                    SKU,
                    Barcode,
                    CategoryId
                );
                await _mediator.Send(command);
            }

            _navigationService.NavigateTo<ProductListViewModel>();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message, "خطأ");
        }
    }

    private void GenerateBarcode()
    {
        try
        {
            // Generate barcode using timestamp and random number
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Random random = new Random();
            int randomPart = random.Next(10000, 99999);
            Barcode = $"{timestamp}{randomPart:D5}".Substring(0, 13); // Standard barcode length
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في إنشاء الباركود: {ex.Message}", "خطأ");
        }
    }
}
