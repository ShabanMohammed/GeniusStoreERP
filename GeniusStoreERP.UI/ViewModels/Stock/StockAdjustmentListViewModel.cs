using GeniusStoreERP.Application.Common;
using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Stock.Adjustments.Queries.GetStockAdjustments;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Stock;

public class StockAdjustmentListViewModel : BaseViewModel
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
                CurrentPage = 1;
                _ = LoadAdjustmentsAsync();
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

    public ObservableCollection<StockAdjustmentDto> Adjustments { get; } = new();

    private StockAdjustmentDto? _selectedAdjustment;
    public StockAdjustmentDto? SelectedAdjustment
    {
        get => _selectedAdjustment;
        set => SetProperty(ref _selectedAdjustment, value);
    }

    public ICommand AddCommand { get; }
    public ICommand ViewDetailsCommand { get; }
    public ICommand ReloadCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }

    public StockAdjustmentListViewModel(IMediator mediator, INavigationService navigationService)
    {
        _mediator = mediator;
        _navigationService = navigationService;

        AddCommand = new RelayCommand(_ => _navigationService.NavigateTo<StockAdjustmentEditorViewModel>());
        ViewDetailsCommand = new RelayCommand(p => {
            if (p is StockAdjustmentDto dto)
                _navigationService.NavigateTo<StockAdjustmentEditorViewModel>(dto.Id);
        });
        ReloadCommand = new RelayCommand(_ => _ = LoadAdjustmentsAsync());
        
        NextPageCommand = new RelayCommand(_ => { 
            if (CurrentPage < TotalPages) { CurrentPage++; _ = LoadAdjustmentsAsync(); } 
        });
        PreviousPageCommand = new RelayCommand(_ => { 
            if (CurrentPage > 1) { CurrentPage--; _ = LoadAdjustmentsAsync(); } 
        });
    }

    public override async void Initialize(object? parameter)
    {
        await LoadAdjustmentsAsync();
    }

    private async Task LoadAdjustmentsAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        try
        {
            var query = new GetStockAdjustmentsQuery(CurrentPage, PageSize, SearchText);
            var result = await _mediator.Send(query);

            Adjustments.Clear();
            foreach (var item in result.Items)
            {
                Adjustments.Add(item);
            }

            TotalItems = result.TotalCount;
            OnPropertyChanged(nameof(TotalPages));
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError($"خطأ في تحميل تسويات المخزن: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
