using GeniusStoreERP.UI.Common;
using System.Windows.Input;
using GeniusStoreERP.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using GeniusStoreERP.Application.Categories.Commands.CreateCategory;
using GeniusStoreERP.Application.Categories.Commands.UpdateCategory;
using GeniusStoreERP.Application.Dtos;
using System.Threading.Tasks;

namespace GeniusStoreERP.UI.ViewModels;

public class CategoryEditViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private int _id;
    private string _name = string.Empty;
    private string? _description;
    private string _title = "إضافة تصنيف جديد";

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

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public CategoryEditViewModel()
        : this(App.ServiceProvider.GetRequiredService<INavigationService>(),
               App.ServiceProvider.GetRequiredService<IMediator>())
    {
    }

    public CategoryEditViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;

        SaveCommand = new AsyncRelayCommand((_, _) => SaveAsync());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<CategoryListViewModel>());
    }

    public override void Initialize(object? parameter)
    {
        if (parameter is CategoryDto category)
        {
            Id = category.Id;
            Name = category.Name;
            Description = category.Description;
            Title = "تعديل التصنيف";
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (Id == 0)
            {
                var command = new CreateCategoryCommand(Name, Description);
                await _mediator.Send(command);
            }
            else
            {
                var command = new UpdateCategoryCommand(Id, Name, Description);
                await _mediator.Send(command);
            }

            _navigationService.NavigateTo<CategoryListViewModel>();
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message , "خطأ");
        }
    }
}
