using GeniusStoreERP.Application.Dtos;
using GeniusStoreERP.Application.Exceptions;
using GeniusStoreERP.Application.Partners.Commands.CreatePartner;
using GeniusStoreERP.Application.Partners.Commands.RestorePartner;
using GeniusStoreERP.Application.Partners.Commands.UpdatePartner;
using GeniusStoreERP.Application.Partners.Commands.UpgradePartnerRole;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels.Partners;

public class PartnerEditViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;

    private int _id;
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _address = string.Empty;
    private bool _isSupplier;
    private bool _isCustomer;
    private string _title = "إضافة شريك جديد";

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

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public bool IsSupplier
    {
        get => _isSupplier;
        set => SetProperty(ref _isSupplier, value);
    }

    public bool IsCustomer
    {
        get => _isCustomer;
        set => SetProperty(ref _isCustomer, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public PartnerEditViewModel()
        : this(App.ServiceProvider.GetRequiredService<INavigationService>(),
               App.ServiceProvider.GetRequiredService<IMediator>())
    {
    }

    public PartnerEditViewModel(INavigationService navigationService, IMediator mediator)
    {
        _navigationService = navigationService;
        _mediator = mediator;

        SaveCommand = new AsyncRelayCommand((_, _) => SaveAsync());
        CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<PartnerListViewModel>());
    }

    public override void Initialize(object? parameter)
    {
        if (parameter is PartnerDto partner)
        {
            Id = partner.Id;
            Name = partner.Name;
            Email = partner.Email;
            PhoneNumber = partner.PhoneNumber;
            Address = partner.Address;
            IsSupplier = partner.IsSupplier;
            IsCustomer = partner.IsCustomer;

            if (Id == 0)
            {
                Title = IsSupplier && IsCustomer ? "إضافة شريك جديد"
                        : IsSupplier ? "إضافة مورد جديد"
                        : "إضافة عميل جديد";
            }
            else
            {
                Title = IsSupplier && IsCustomer ? "تعديل بيانات الشريك"
                        : IsSupplier ? "تعديل بيانات المورد"
                        : "تعديل بيانات العميل";
            }
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            if (Id == 0)
            {
                var command = new CreatePartnerCommand(Name, Email, PhoneNumber, Address, IsSupplier, IsCustomer);
                await _mediator.Send(command);
            }
            else
            {
                var command = new UpdatePartnerCommand(Id, Name, Email, PhoneNumber, Address, IsCustomer, IsSupplier);
                await _mediator.Send(command);
            }
            if (IsCustomer)
                _navigationService.NavigateTo<PartnerListViewModel>("Customers");
            else
                _navigationService.NavigateTo<PartnerListViewModel>("Suppliers");
        }
        catch (EntityDeletedException deletedEx)
        {
            if (deletedEx.Entity is GeniusStoreERP.Domain.Entities.Partners.Partner partner)
            {
                var result = MessageBoxService.ShowConfirmation($"الاسم '{Name}' موجود مسبقاً في سلة المحذوفات. هل تريد استعادته؟");
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    await _mediator.Send(new RestorePartnerCommand(partner.Id));
                    if (IsCustomer)
                        _navigationService.NavigateTo<PartnerListViewModel>("Customers");
                    else
                        _navigationService.NavigateTo<PartnerListViewModel>("Suppliers");
                }
            }
        }
        catch (EntityConflictException conflictEx)
        {
            if (conflictEx.Entity is GeniusStoreERP.Domain.Entities.Partners.Partner partner)
            {
                string targetRole = IsCustomer ? "عميل" : "مورد";
                var result = MessageBoxService.ShowConfirmation($"هذا الشريك موجود مسبقاً. هل تريد إضافة صفة '{targetRole}' له بدلاً من إضافة شريك جديد؟");
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    await _mediator.Send(new UpgradePartnerRoleCommand(partner.Id));
                    if (IsCustomer)
                        _navigationService.NavigateTo<PartnerListViewModel>("Customers");
                    else
                        _navigationService.NavigateTo<PartnerListViewModel>("Suppliers");
                }
            }
        }
        catch (BusinessException)
        {
            MessageBoxService.ShowError("هذا الشريك موجود مسبقاً بنفس الصفة.");
        }
        catch (Exception ex)
        {
            MessageBoxService.ShowError(ex.Message, "خطأ");
        }
    }
}
