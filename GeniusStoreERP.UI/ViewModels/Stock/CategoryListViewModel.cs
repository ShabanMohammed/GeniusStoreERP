using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Stock.Categories.Commands.DeleteCategory;
using GeniusStoreERP.Application.Stock.Categories.Queries.GetCategories;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Stock;

public class CategoryListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private string _searchText = string.Empty;
    private int _pageSize = 10;
    private int _currentPage = 1;
    private int _totalItems = 0;
    private CategoryDto? _selectedCategory;

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                CurrentPage = 1;
                _ = LoadCategoriesAsync();
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
                _ = LoadCategoriesAsync();
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
                _ = LoadCategoriesAsync();
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

    public CategoryDto? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    public ICommand SearchCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand NextPageCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand FirstPageCommand { get; }
    public ICommand LastPageCommand { get; }
    public ICommand IncreasePageSizeCommand { get; }
    public ICommand DecreasePageSizeCommand { get; }



    public CategoryListViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;
        SearchCommand = new AsyncRelayCommand((_, _) => LoadCategoriesAsync());
        AddCommand = new RelayCommand(_ => _navigationService.NavigateTo<CategoryEditViewModel>());
        EditCommand = new RelayCommand(p =>
        {
            var category = p as CategoryDto ?? SelectedCategory;
            if (category != null)
            {
                _navigationService.NavigateTo<CategoryEditViewModel>(category);
            }
        });
        DeleteCommand = new AsyncRelayCommand((p, _) => DeleteCategory(p as CategoryDto));
        NextPageCommand = new AsyncRelayCommand((_, _) => { if (CurrentPage < TotalPages) CurrentPage++; return Task.CompletedTask; }, _ => CurrentPage < TotalPages);
        PreviousPageCommand = new AsyncRelayCommand((_, _) => { if (CurrentPage > 1) CurrentPage--; return Task.CompletedTask; }, _ => CurrentPage > 1);
        FirstPageCommand = new AsyncRelayCommand((_, _) => { CurrentPage = 1; return Task.CompletedTask; }, _ => CurrentPage > 1);
        LastPageCommand = new AsyncRelayCommand((_, _) => { if (TotalPages > 0) CurrentPage = TotalPages; return Task.CompletedTask; }, _ => CurrentPage < TotalPages);

        IncreasePageSizeCommand = new RelayCommand(_ => PageSize += 1);
        DecreasePageSizeCommand = new RelayCommand(_ => { if (PageSize > 1) PageSize -= 1; });

        _ = LoadCategoriesAsync();

    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var query = new GetCategoriesQuery(SearchText, CurrentPage, PageSize);
            var result = await _mediator.Send(query);

            Categories.Clear();
            if (result?.Items != null)
            {
                foreach (var category in result.Items)
                {
                    Categories.Add(category);
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

    private async Task DeleteCategory(CategoryDto? category)
    {
        if (category == null) return;
        var result = MessageBoxService.ShowConfirmation($"هل أنت متأكد أنك تريد حذف التصنيف '{category.Name}'؟\n فى حالة الحذف لن تتمكن من اضافة تنصيف بنفس الاسم مرة اخرى");
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            var command = new DeleteCategoryCommand(category.Id);
            try
            {
                await _mediator.Send(command);
                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {

                MessageBoxService.ShowError(ex.Message);
            }

        }
    }
}
