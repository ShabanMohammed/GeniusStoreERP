using FluentValidation;
using GeniusStoreERP.Application.Behaviors;
using GeniusStoreERP.Application.Common.Interfaces;
using GeniusStoreERP.Application.Users.Commands.CreateUser;
using GeniusStoreERP.Domain.Entities;
using GeniusStoreERP.Infrastructure.Data;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.UI.ViewModels;
using GeniusStoreERP.UI.ViewModels.Partners;
using GeniusStoreERP.UI.ViewModels.Stock;
using GeniusStoreERP.UI.ViewModels.Transactions;
using GeniusStoreERP.UI.Views;
using GeniusStoreERP.UI.Views.Partners;
using GeniusStoreERP.UI.Views.Transactions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeniusStoreERP.UI
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = default!;
        private IConfiguration? configuration;

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var services = new ServiceCollection();

                services.AddLogging();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
                });

                services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

                // Identity registration
                services
                    .AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

                // MediatR registration
                services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly)
                );
                services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CreateUserCommand).Assembly));

                services.AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly);
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<LoginView>();
                services.AddTransient<LoginViewModel>();

                services.AddSingleton<MainView>();
                services.AddTransient<MainViewModel>();

                services.AddTransient<DashboardView>();
                services.AddTransient<DashboardViewModel>();

                services.AddTransient<CategoryListView>();
                services.AddTransient<CategoryListViewModel>();
                services.AddTransient<CategoryEditView>();
                services.AddTransient<CategoryEditViewModel>();

                services.AddTransient<ProductListView>();
                services.AddTransient<ProductListViewModel>();
                services.AddTransient<ProductEditView>();
                services.AddTransient<ProductEditViewModel>();

                services.AddTransient<PartnerListView>();
                services.AddTransient<PartnerListViewModel>();
                services.AddTransient<PartnerEditView>();
                services.AddTransient<PartnerEditViewModel>();

                services.AddTransient<InvoiceListView>();
                services.AddTransient<InvoiceListViewModel>();
                services.AddTransient<InvoiceEditorView>();
                services.AddTransient<InvoiceEditorViewModel>();
                services.AddTransient<InvoiceDetailsView>();
                services.AddTransient<InvoiceDetailsViewModel>();

                // Settings
                services.AddTransient<GeneralSettingEditView>();
                services.AddTransient<GeneralSettingEditViewModel>();

                services.AddSingleton<IInvoiceReportService, InvoiceReportService>();
                
                services.AddTransient<ReportOptionsWindow>();
                services.AddTransient<ReportOptionsViewModel>();
                services.AddTransient<ReportPreviewWindow>();
                services.AddTransient<ReportPreviewViewModel>();

                // 7. بناء الـ ServiceProvider النهائي
                ServiceProvider = services.BuildServiceProvider();

                await InitializeDatabaseAsync();

                var mainWindow = ServiceProvider.GetRequiredService<LoginView>();
                mainWindow.Show();

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred during startup: {ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Shutdown();
            }
        }

        private async Task InitializeDatabaseAsync()
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    FullName = "Administrator",
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (!await context.GeneralSettings.AnyAsync())
            {
                var generalSettings = new GeneralSetting
                {
                    CompanyName = "Genius Store ERP",
                    TaxPercentage = 14,
                    CurrencySymbol = "EGP",
                };

                await context.GeneralSettings.AddAsync(generalSettings);
                await context.SaveChangesAsync();

            }

        }

        private void SelectAllText(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        // دالة تجعل الماوس يركز على الحقل دون تحريك المؤشر في أول نقرة
        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                textBox.Focus();
            }
        }
    }
}
