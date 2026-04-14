using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Products.Queries.GetProductById;
using GeniusStoreERP.Application.Stock.Adjustments.Commands.CreateStockAdjustment;
using GeniusStoreERP.Application.Stock.Adjustments.Queries.GetStockAdjustmentById;
using GeniusStoreERP.Application.Stock.Products.Queries.GetProducts;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Stock;

public class StockAdjustmentEditorViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly INavigationService _navigationService;

    // View state
    private bool _isViewMode;
    public bool IsViewMode
    {
        get => _isViewMode;
        set => SetProperty(ref _isViewMode, value);
    }

    private string _title = "إضافة تسوية جديدة";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // Header properties
    private DateTime _adjustmentDate = DateTime.Now;
    public DateTime AdjustmentDate
    {
        get => _adjustmentDate;
        set => SetProperty(ref _adjustmentDate, value);
    }

    private string _referenceNumber = "<تلقائي>";
    public string ReferenceNumber
    {
        get => _referenceNumber;
        set => SetProperty(ref _referenceNumber, value);
    }

    private string? _remarks;
    public string? Remarks
    {
        get => _remarks;
        set => SetProperty(ref _remarks, value);
    }

    // Product selection
    private string _productSearchText = string.Empty;
    public string ProductSearchText
    {
        get => _productSearchText;
        set => SetProperty(ref _productSearchText, value);
    }

    private ObservableCollection<ProductDto> _searchResults = new();
    public ObservableCollection<ProductDto> SearchResults
    {
        get => _searchResults;
        set => SetProperty(ref _searchResults, value);
    }

    private ProductDto? _selectedProduct;
    public ProductDto? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value) && value != null)
            {
                AddProductToAdjustment(value);
                ProductSearchText = string.Empty;
                SearchResults.Clear();
            }
        }
    }

    // Transaction Types (2 = تسوية, 3 = تلف)
    public ObservableCollection<StockTransactionTypeDto> TransactionTypes { get; } = new()
    {
        new StockTransactionTypeDto { Id = 2, Name = "تسوية" },
        new StockTransactionTypeDto { Id = 3, Name = "تلف" }
    };

    // Items
    public ObservableCollection<StockAdjustmentItemViewModel> Items { get; } = new();

    public ICommand BackCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SearchProductCommand { get; }
    public ICommand RemoveItemCommand { get; }

    public StockAdjustmentEditorViewModel(IMediator mediator, INavigationService navigationService)
    {
        _mediator = mediator;
        _navigationService = navigationService;

        BackCommand = new RelayCommand(_ => _navigationService.NavigateTo<StockAdjustmentListViewModel>());
        SaveCommand = new AsyncRelayCommand(async (_, _) => await SaveAsync(), _ => CanSave());
        SearchProductCommand = new AsyncRelayCommand(async (_, _) => await SearchProductsAsync());
        RemoveItemCommand = new RelayCommand(p => {
            if (p is StockAdjustmentItemViewModel item && !IsViewMode)
            {
                Items.Remove(item);
                (SaveCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        });
    }

    public override async void Initialize(object? parameter)
    {
        if (parameter is int adjustmentId)
        {
            IsViewMode = true;
            Title = "عرض تفاصيل التسوية";
            await LoadAdjustmentAsync(adjustmentId);
        }
        else
        {
            IsViewMode = false;
            Title = "إضافة تسوية جديدة";
            AdjustmentDate = DateTime.Now;
            ReferenceNumber = "<تلقائي>";
            Remarks = string.Empty;
            Items.Clear();
        }
    }

    private async Task LoadAdjustmentAsync(int id)
    {
        try
        {
            var dto = await _mediator.Send(new GetStockAdjustmentByIdQuery(id));
            if (dto != null)
            {
                AdjustmentDate = dto.AdjustmentDate;
                ReferenceNumber = dto.ReferenceNumber;
                Remarks = dto.Remarks;

                Items.Clear();
                foreach (var item in dto.Items)
                {
                    Items.Add(new StockAdjustmentItemViewModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        PreviousQuantity = item.PreviousQuantity,
                        QuantityChange = item.QuantityChange,
                        SelectedTransactionType = TransactionTypes.FirstOrDefault(t => t.Id == item.StockTransactionTypeId)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في تحميل بيانات التسوية: {ex.Message}");
        }
    }

    private async Task SearchProductsAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductSearchText) || ProductSearchText.Length < 2)
        {
            SearchResults.Clear();
            return;
        }

        try
        {
            var result = await _mediator.Send(new GetProductsQuery(ProductSearchText, null, 1, 10));
            SearchResults.Clear();
            if (result?.Items != null)
            {
                foreach (var p in result.Items) SearchResults.Add(p);
            }
        }
        catch { } // Ignore search errors
    }

    private void AddProductToAdjustment(ProductDto product)
    {
        if (Items.Any(i => i.ProductId == product.Id))
        {
            MessageBoxService.ShowWarning("هذا الصنف مضاف مسبقاً في القائمة.");
            return;
        }

        var item = new StockAdjustmentItemViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name,
            PreviousQuantity = product.StockQuantity ?? 0,
            QuantityChange = 0,
            SelectedTransactionType = TransactionTypes.First() // Default to تسوية
        };
        
        item.PropertyChanged += (s, e) => {
            if (e.PropertyName == nameof(StockAdjustmentItemViewModel.QuantityChange))
            {
                (SaveCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        };

        Items.Add(item);
        (SaveCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool CanSave()
    {
        if (IsViewMode) return false;
        if (!Items.Any()) return false;
        if (Items.Any(i => i.QuantityChange == 0)) return false; // Must have some change
        if (Items.Any(i => i.SelectedTransactionType == null)) return false;
        
        return true;
    }

    private async Task SaveAsync()
    {
        try
        {
            var command = new CreateStockAdjustmentCommand
            {
                AdjustmentDate = AdjustmentDate,
                Remarks = Remarks,
                Items = Items.Select(i => new CreateStockAdjustmentItemCommand
                {
                    ProductId = i.ProductId,
                    QuantityChange = i.QuantityChange,
                    StockTransactionTypeId = i.SelectedTransactionType!.Id
                }).ToList()
            };

            var id = await _mediator.Send(command);
            MessageBoxService.ShowSuccess("تم إضافة التسوية بنجاح.");
            _navigationService.NavigateTo<StockAdjustmentListViewModel>();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في الحفظ: {ex.Message}");
        }
    }
}

public class StockTransactionTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class StockAdjustmentItemViewModel : BaseViewModel
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    
    private decimal _previousQuantity;
    public decimal PreviousQuantity
    {
        get => _previousQuantity;
        set
        {
            if (SetProperty(ref _previousQuantity, value))
                OnPropertyChanged(nameof(NewQuantity));
        }
    }

    private decimal _quantityChange;
    public decimal QuantityChange
    {
        get => _quantityChange;
        set
        {
            if (SetProperty(ref _quantityChange, value))
                OnPropertyChanged(nameof(NewQuantity));
        }
    }

    public decimal NewQuantity => PreviousQuantity + QuantityChange;

    private StockTransactionTypeDto? _selectedTransactionType;
    public StockTransactionTypeDto? SelectedTransactionType
    {
        get => _selectedTransactionType;
        set => SetProperty(ref _selectedTransactionType, value);
    }
}
