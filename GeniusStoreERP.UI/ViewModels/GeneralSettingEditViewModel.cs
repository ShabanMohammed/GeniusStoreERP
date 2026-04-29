using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.UI.Models;
using GeniusStoreERP.Application.GeneralSettings.Commands;
using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;
using GeniusStoreERP.Application.Dtos;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

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
        public ICommand BackupDatabaseCommand { get; }
        public ICommand RestoreDatabaseCommand { get; }

        public GeneralSettingEditViewModel(INavigationService navigationService, IMediator mediator)
        {
            _navigationService = navigationService;
            _mediator = mediator;

            _settingModel = new GeneralSettingEditModel();

            SaveCommand = new AsyncRelayCommand((_, _) => OnSaveAsync());
            CancelCommand = new RelayCommand(_ => _navigationService.NavigateTo<DashboardViewModel>());
            SelectLogoCommand = new RelayCommand(OnSelectLogo);
            RemoveLogoCommand = new RelayCommand(OnRemoveLogo);
            BackupDatabaseCommand = new AsyncRelayCommand((_, _) => OnBackupDatabaseAsync());
            RestoreDatabaseCommand = new AsyncRelayCommand((_, _) => OnRestoreDatabaseAsync());
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

        private async Task OnBackupDatabaseAsync()
        {
            if (IsLoading) return;

            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "حفظ نسخة احتياطية",
                Filter = "SQLite Database|*.db;*.sqlite|All Files|*.*",
                FileName = $"GeniusStore_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db"
            };

            if (saveDialog.ShowDialog() != true) return;

            IsLoading = true;
            try
            {
                var appDbPath = GetDatabasePath();
                await Task.Run(() => BackupDatabase(appDbPath, saveDialog.FileName));
                MessageBoxService.ShowSuccess("تم حفظ النسخة الاحتياطية بنجاح");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"فشل حفظ النسخة الاحتياطية: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnRestoreDatabaseAsync()
        {
            if (IsLoading) return;

            var confirmation = MessageBoxService.ShowConfirmation(
                "سيتم استبدال بيانات النظام الحالية بالنسخة المختارة. هل تريد المتابعة؟",
                "تأكيد الاسترجاع");

            if (confirmation != System.Windows.MessageBoxResult.Yes) return;

            var openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "اختيار النسخة الاحتياطية",
                Filter = "SQLite Database|*.db;*.sqlite|All Files|*.*"
            };

            if (openDialog.ShowDialog() != true) return;

            IsLoading = true;
            try
            {
                var appDbPath = GetDatabasePath();
                await Task.Run(() => RestoreDatabase(openDialog.FileName, appDbPath));
                MessageBoxService.ShowSuccess("تم استرجاع النسخة الاحتياطية بنجاح");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"فشل استرجاع النسخة الاحتياطية: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static string GetDatabasePath()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' غير موجود.");

            var builder = new SqliteConnectionStringBuilder(connectionString);
            var dataSource = builder.DataSource;

            if (string.IsNullOrWhiteSpace(dataSource))
                throw new InvalidOperationException("Data Source غير معرف في Connection String.");

            return Path.IsPathRooted(dataSource)
                ? dataSource
                : Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataSource));
        }

        private static void BackupDatabase(string sourceDbPath, string destinationBackupPath)
        {
            if (!File.Exists(sourceDbPath))
                throw new FileNotFoundException("ملف قاعدة البيانات غير موجود.", sourceDbPath);

            var destinationDirectory = Path.GetDirectoryName(destinationBackupPath);
            if (!string.IsNullOrWhiteSpace(destinationDirectory) && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            using var sourceConnection = new SqliteConnection($"Data Source={sourceDbPath}");
            using var destinationConnection = new SqliteConnection($"Data Source={destinationBackupPath}");
            sourceConnection.Open();
            destinationConnection.Open();

            sourceConnection.BackupDatabase(destinationConnection);
        }

        private static void RestoreDatabase(string backupDbPath, string destinationDbPath)
        {
            if (!File.Exists(backupDbPath))
                throw new FileNotFoundException("ملف النسخة الاحتياطية غير موجود.", backupDbPath);

            var destinationDirectory = Path.GetDirectoryName(destinationDbPath);
            if (!string.IsNullOrWhiteSpace(destinationDirectory) && !Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            using var sourceConnection = new SqliteConnection($"Data Source={backupDbPath}");
            using var destinationConnection = new SqliteConnection($"Data Source={destinationDbPath}");
            sourceConnection.Open();
            destinationConnection.Open();

            sourceConnection.BackupDatabase(destinationConnection);
        }
    }
}

