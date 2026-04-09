using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.UI.Models;
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
        private GeneralSettingEditModel? _settingModel;
        private bool _isLoading;

        public GeneralSettingEditModel? SettingModel
        {
            get => _settingModel;
            set => SetProperty(ref _settingModel, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectLogoCommand { get; }
        public ICommand RemoveLogoCommand { get; }

        public GeneralSettingEditViewModel(INavigationService navigationService, IMediator mediator)
        {
            _navigationService = navigationService;
            _mediator = mediator;

            _settingModel = new GeneralSettingEditModel();

            SaveCommand = new AsyncRelayCommand((_, _) => OnSaveAsync());
            CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<DashboardViewModel>());
            SelectLogoCommand = new RelayCommand(OnSelectLogo);
            RemoveLogoCommand = new RelayCommand(OnRemoveLogo);
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
                    SettingModel = new GeneralSettingEditModel
                    {
                        CompanyName = result.CompanyName,
                        LegalName = result.LegalName,
                        Address = result.Address,
                        Phone1 = result.Phone1,
                        Phone2 = result.Phone2,
                        Email = result.Email,
                        Website = result.Website,
                        TaxNumber = result.TaxNumber,
                        Logo = result.Logo,
                        TaxPercentage = result.TaxPercentage,
                        CurrencySymbol = result.CurrencySymbol
                    };
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
            if (SettingModel == null || IsLoading) return;

            if (!SettingModel.IsValid())
            {
                MessageBoxService.ShowWarning("يرجى التأكد من إدخال اسم الشركة");
                return;
            }

            IsLoading = true;
            try
            {
                var command = new GeneralSettingUpdateCommand(
                    SettingModel.CompanyName,
                    SettingModel.LegalName,
                    SettingModel.Address,
                    SettingModel.Phone1,
                    SettingModel.Phone2,
                    SettingModel.Email,
                    SettingModel.Website,
                    SettingModel.TaxNumber,
                    SettingModel.Logo,
                    SettingModel.TaxPercentage,
                    SettingModel.CurrencySymbol
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

        private void OnSelectLogo(object? parameter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "اختر شعار الشركة"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    byte[] logoBytes = System.IO.File.ReadAllBytes(dialog.FileName);
                    if (SettingModel != null)
                    {
                        SettingModel.Logo = logoBytes;
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxService.ShowError($"فشل في قراءة ملف الصورة: {ex.Message}");
                }
            }
        }

        private void OnRemoveLogo(object? parameter)
        {
            if (SettingModel != null)
            {
                SettingModel.Logo = null;
            }
        }
    }
}

