using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Dtos.ListItemDto;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;
using GeniusStoreERP.Application.Partners.Queries.GetPartnerItems;
using GeniusStoreERP.Application.Stock.Products.Queries.GetProductById;
using GeniusStoreERP.Application.Stock.Products.Queries.GetProductItems;
using GeniusStoreERP.Application.Transactions.Commands.CreatePurchaseInvoice;
using GeniusStoreERP.Application.Transactions.Commands.CreateReturnPurchaseInvoice;
using GeniusStoreERP.Application.Transactions.Commands.CreateReturnSalesInvoice;
using GeniusStoreERP.Application.Transactions.Commands.CreateSalesInvoice;
using GeniusStoreERP.Domain.Enums;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Transactions;

public class InvoiceEditorViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly INavigationService _navigationService;

    private int _invoiceTypeId;
    private DateTime _invoiceDate = DateTime.UtcNow;
    private PartnerListItemDto? _selectedPartner;
    private string? _notes;
    private ObservableCollection<PartnerListItemDto> _partners = new();
    private ObservableCollection<ProductListItemDto> _products = new();
    private ObservableCollection<InvoiceItemEditor> _invoiceItems = new();
    private InvoiceItemEditor _currentItem = new();
    private ProductListItemDto? _selectedProduct;
    private decimal _taxPercentageFromSettings;

    public DateTime InvoiceDate { get => _invoiceDate; set => SetProperty(ref _invoiceDate, value); }
    public PartnerListItemDto? SelectedPartner { get => _selectedPartner; set => SetProperty(ref _selectedPartner, value); }
    public string? Notes { get => _notes; set => SetProperty(ref _notes, value); }
    public ObservableCollection<PartnerListItemDto> Partners { get => _partners; set => SetProperty(ref _partners, value); }
    public ObservableCollection<ProductListItemDto> Products { get => _products; set => SetProperty(ref _products, value); }
    public ObservableCollection<InvoiceItemEditor> InvoiceItems { get => _invoiceItems; set => SetProperty(ref _invoiceItems, value); }

    public InvoiceItemEditor CurrentItem { get => _currentItem; set => SetProperty(ref _currentItem, value); }
    public ProductListItemDto? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value))
            {
                if (value != null)
                {
                    CurrentItem.ProductId = value.Id;
                    CurrentItem.ProductName = value.Name;
                    if (_invoiceTypeId == (int)InvoiceTypeEnum.Sales || _invoiceTypeId == (int)InvoiceTypeEnum.ReturnSales)
                        CurrentItem.UnitPrice = value.Price;
                }
            }
        }
    }

    public decimal TotalAmount => InvoiceItems.Sum(x => x.LineTotal);
    public decimal TotalDiscount => InvoiceItems.Sum(x => x.DiscountAmount);
    public decimal TotalTax => InvoiceItems.Sum(x => x.TaxAmount);
    public decimal FinalAmount => TotalAmount - TotalDiscount + TotalTax;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }

    public InvoiceEditorViewModel(IMediator mediator, INavigationService navigationService)
    {
        _mediator = mediator;
        _navigationService = navigationService;

        SaveCommand = new AsyncRelayCommand(async (param, _) => await SaveInvoice());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<InvoiceListViewModel>(_invoiceTypeId));
        AddItemCommand = new RelayCommand(_ => AddNewItem());
        RemoveItemCommand = new RelayCommand(item => RemoveItem(item as InvoiceItemEditor));
    }

    public override async void Initialize(object? parameter)
    {
        if (parameter is int typeId)
        {
            _invoiceTypeId = typeId;
        }
        else if (parameter is InvoiceDto invoice)
        {
            // Edit mode logic (not implemented yet)
        }

        await LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            // تحميل الشركاء (عملاء أو موردين) بناءً على نوع الفاتورة
            bool isCustomer = _invoiceTypeId == (int)InvoiceTypeEnum.Sales || _invoiceTypeId == (int)InvoiceTypeEnum.ReturnSales;
            bool isSupplier = _invoiceTypeId == (int)InvoiceTypeEnum.Purchase || _invoiceTypeId == (int)InvoiceTypeEnum.ReturnPurchase;

            var partnersResult = await _mediator.Send(new GetPartnerItemsCommand(isCustomer, isSupplier));
            Partners = new ObservableCollection<PartnerListItemDto>(partnersResult);

            var productsResult = await _mediator.Send(new GetProductItemsCommand());
            Products = new ObservableCollection<ProductListItemDto>(productsResult);

            var settingsResult = await _mediator.Send(new GetGeneralSettingsQuery());
            if (settingsResult != null)
            {
                _taxPercentageFromSettings = settingsResult.TaxPercentage;
                CurrentItem.TaxPercentage = _taxPercentageFromSettings;
            }
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في تحميل البيانات: {ex.Message}");
        }
    }

    private async Task AddNewItem()
    {
        if (SelectedProduct == null)
        {
            MessageBoxService.ShowWarning("يرجى اختيار المنتج أولاً");
            return;
        }

        if (CurrentItem.Quantity <= 0)
        {
            MessageBoxService.ShowWarning("يجب أن تكون الكمية أكبر من صفر");
            return;
        }

        if (CurrentItem.UnitPrice < 0)
        {
            MessageBoxService.ShowWarning("يجب أن يكون السعر صحيحاً");
            return;
        }
        if (_invoiceTypeId == (int)InvoiceTypeEnum.Sales || _invoiceTypeId == (int)InvoiceTypeEnum.ReturnPurchase)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(CurrentItem.ProductId));
            if (product.StockQuantity < CurrentItem.Quantity)
            {
                MessageBoxService.ShowWarning($"لا يوجد كمية كافية للمنتج في المخزن \n كمية المنتج في المخزن الحالى {product.StockQuantity}");
                return;
            }
        }
        var newItem = new InvoiceItemEditor
        {
            ProductId = CurrentItem.ProductId,
            ProductName = CurrentItem.ProductName,
            Quantity = CurrentItem.Quantity,
            UnitPrice = CurrentItem.UnitPrice,
            DiscountPercentage = CurrentItem.DiscountPercentage,
            DiscountAmount = CurrentItem.DiscountAmount,
            TaxPercentage = CurrentItem.TaxPercentage,
            TaxAmount = CurrentItem.TaxAmount
        };

        newItem.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(InvoiceItemEditor.LineTotal) ||
                e.PropertyName == nameof(InvoiceItemEditor.DiscountAmount) ||
                e.PropertyName == nameof(InvoiceItemEditor.TaxAmount))
            {
                UpdateTotals();
            }
        };

        InvoiceItems.Add(newItem);
        UpdateTotals();

        CurrentItem = new InvoiceItemEditor();
        SelectedProduct = null;
    }

    private void RemoveItem(InvoiceItemEditor? item)
    {
        if (item != null)
        {
            InvoiceItems.Remove(item);
            UpdateTotals();
        }
    }

    private void UpdateTotals()
    {
        OnPropertyChanged(nameof(TotalAmount));
        OnPropertyChanged(nameof(TotalDiscount));
        OnPropertyChanged(nameof(TotalTax));
        OnPropertyChanged(nameof(FinalAmount));
    }

    private async Task SaveInvoice()
    {
        if (SelectedPartner == null)
        {
            string partnerType = (_invoiceTypeId == (int)InvoiceTypeEnum.Purchase || _invoiceTypeId == (int)InvoiceTypeEnum.ReturnPurchase) ? "المورد" : "العميل";
            MessageBoxService.ShowWarning($"يرجى اختيار {partnerType} أولاً");
            return;
        }

        if (!InvoiceItems.Any())
        {
            MessageBoxService.ShowWarning("لا يمكن حفظ فاتورة فارغة");
            return;
        }

        try
        {
            var items = InvoiceItems.Select(x => new InvoiceItemDto(
                    x.ProductId,
                    x.ProductName,
                    (int)x.Quantity,
                    x.UnitPrice,
                    x.DiscountPercentage,
                    x.DiscountAmount,
                    x.TaxPercentage,
                    x.TaxAmount,
                    x.LineTotal
                )).ToList();

            object command = _invoiceTypeId switch
            {
                (int)InvoiceTypeEnum.Sales => new CreateSalesInvoiceCommand(
                    InvoiceDate, TotalAmount, TotalDiscount, TotalTax, FinalAmount, Notes, SelectedPartner.Id, 1, items),

                (int)InvoiceTypeEnum.Purchase => new CreatePurchaseInvoiceCommand(
                    InvoiceDate, TotalAmount, TotalDiscount, TotalTax, FinalAmount, Notes, SelectedPartner.Id, 1, items),

                (int)InvoiceTypeEnum.ReturnSales => new CreateReturnSalesInvoiceCommand(
                    InvoiceDate, TotalAmount, TotalDiscount, TotalTax, FinalAmount, Notes, SelectedPartner.Id, 1, items),

                (int)InvoiceTypeEnum.ReturnPurchase => new CreateReturnPurchaseInvoiceCommand(
                    InvoiceDate, TotalAmount, TotalDiscount, TotalTax, FinalAmount, Notes, SelectedPartner.Id, 1, items),

                _ => throw new BusinessException("نوع فاتورة غير معروف")
            };

            await _mediator.Send(command);
            MessageBoxService.ShowSuccess("تم حفظ الفاتورة بنجاح");
            _navigationService.NavigateTo<InvoiceListViewModel>(_invoiceTypeId);
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message);
        }
    }
}

public class InvoiceItemEditor : BaseViewModel
{
    private int _productId;
    private string _productName = string.Empty;
    private decimal _quantity = 1;
    private decimal _unitPrice;
    private decimal _discountPercentage;
    private decimal _discountAmount;
    private decimal _taxPercentage;
    private decimal _taxAmount;

    public int ProductId { get => _productId; set => SetProperty(ref _productId, value); }
    public string ProductName { get => _productName; set => SetProperty(ref _productName, value); }

    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
                RecalculateAmounts();
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (SetProperty(ref _unitPrice, value))
                RecalculateAmounts();
        }
    }

    public decimal DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            if (SetProperty(ref _discountPercentage, value))
            {
                DiscountAmount = (Quantity * UnitPrice) * (value / 100);
            }
        }
    }

    public decimal DiscountAmount
    {
        get => _discountAmount;
        set
        {
            if (SetProperty(ref _discountAmount, value))
            {
                OnPropertyChanged(nameof(LineTotal));
                if (TaxPercentage > 0)
                {
                    TaxAmount = ((Quantity * UnitPrice) - value) * (TaxPercentage / 100);
                }
            }
        }
    }

    public decimal TaxPercentage
    {
        get => _taxPercentage;
        set
        {
            if (SetProperty(ref _taxPercentage, value))
            {
                TaxAmount = ((Quantity * UnitPrice) - DiscountAmount) * (value / 100);
            }
        }
    }

    public decimal TaxAmount
    {
        get => _taxAmount;
        set
        {
            if (SetProperty(ref _taxAmount, value))
            {
                OnPropertyChanged(nameof(LineTotal));
            }
        }
    }

    public decimal LineTotal => (Quantity * UnitPrice) - DiscountAmount + TaxAmount;

    private void RecalculateAmounts()
    {
        if (DiscountPercentage > 0)
            DiscountAmount = (Quantity * UnitPrice) * (DiscountPercentage / 100);

        if (TaxPercentage > 0)
            TaxAmount = ((Quantity * UnitPrice) - DiscountAmount) * (TaxPercentage / 100);

        OnPropertyChanged(nameof(LineTotal));
    }
}