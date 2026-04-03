using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.Application.GeneralSettings.Commands;
using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;
using GeniusStoreERP.Application.Dtos;
using MediatR;

namespace GeniusStoreERP.UI.ViewModels
{
    public class GeneralSettingEditViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IMediator _mediator;
        private GeneralSettingsDto? _generalSetting;
        private bool _isLoading;

        public GeneralSettingsDto? GeneralSetting
        {
            get => _generalSetting;
            set => SetProperty(ref _generalSetting, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public GeneralSettingEditViewModel(INavigationService navigationService, IMediator mediator)
        {
            _navigationService = navigationService;
            _mediator = mediator;

            _generalSetting = new GeneralSettingsDto(
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                string.Empty
            );

            SaveCommand = new AsyncRelayCommand((_, _) => OnSaveAsync());
            CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<DashboardViewModel>());
        }

        public override async void Initialize(object? parameter)
        {
            await LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            IsLoading = true;
            try
            {
                var query = new GetGeneralSettingsQuery();
                var result = await _mediator.Send(query);
                if (result != null)
                {
                    GeneralSetting = result;
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"خطأ في تحميل الإعدادات: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnSaveAsync()
        {
            if (GeneralSetting == null || IsLoading) return;

            IsLoading = true;
            try
            {
                var command = new GeneralSettingUpdateCommand(
                    GeneralSetting.CompanyName,
                    GeneralSetting.LegalName,
                    GeneralSetting.Address,
                    GeneralSetting.Phone1,
                    GeneralSetting.Phone2,
                    GeneralSetting.Email,
                    GeneralSetting.Website,
                    GeneralSetting.TaxNumber,
                    GeneralSetting.Logo,
                    GeneralSetting.TaxPercentage,
                    GeneralSetting.CurrencySymbol
                );

                await _mediator.Send(command);
                MessageBoxService.ShowSuccess("تم حفظ الإعدادات بنجاح");
                _navigationService.NavigateTo<DashboardViewModel>();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"حدث خطأ أثناء الحفظ: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
