using GeniusStoreERP.UI.Common;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GeniusStoreERP.UI.ViewModels.Partners;
using GeniusStoreERP.UI.ViewModels.Stock;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.Application.Stock.Products.Queries.GetLowStockProducts;
using GeniusStoreERP.Application.Stock.Products.Queries.GetAllProducts;
using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;
using GeniusStoreERP.Application.Common.Interfaces;
using MediatR;
using System.Threading.Tasks;
using System;
using GeniusStoreERP.UI.Views;
using System.Windows;
using GeniusStoreERP.Application.Partners.Queries.GetDebtAging;
using GeniusStoreERP.Application.Partners.Queries.GetPartnerAccounts;

namespace GeniusStoreERP.UI.ViewModels;

public class ReportsMainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private readonly IStockReportService _stockReportService;
    private readonly IPartnerReportService _partnerReportService;

    public ObservableCollection<ReportCategory> Categories { get; } = new();

    public ReportsMainViewModel()
        : this(
            App.ServiceProvider.GetRequiredService<INavigationService>(),
            App.ServiceProvider.GetRequiredService<IMediator>(),
            App.ServiceProvider.GetRequiredService<IStockReportService>(),
            App.ServiceProvider.GetRequiredService<IPartnerReportService>())
    {
    }

    public ReportsMainViewModel(
        INavigationService navigationService, 
        IMediator mediator,
        IStockReportService stockReportService,
        IPartnerReportService partnerReportService)
    {
        _navigationService = navigationService;
        _mediator = mediator;
        _stockReportService = stockReportService;
        _partnerReportService = partnerReportService;
        InitializeCategories();
    }

    private void InitializeCategories()
    {
        // 1. تقارير المخازن
        var stockCategory = new ReportCategory 
        { 
            Title = "تقارير المخازن", 
            IconKey = "IconInformation",
            Description = "متابعة حركة الأرصدة، جرد المخزن، وتنبيهات النواقص."
        };
        stockCategory.Reports.Add(new ReportItem 
        { 
            Title = "جرد المخزون الحالي", 
            Description = "عرض الكميات الحالية لكل صنف وقيمتها.",
            OpenCommand = new AsyncRelayCommand(async (p, ct) => await GenerateInventoryValueReport())
        });
        stockCategory.Reports.Add(new ReportItem 
        { 
            Title = "الأصناف منخفضة المخزون", 
            Description = "الأصناف التي وصلت لحد الطلب.",
            OpenCommand = new AsyncRelayCommand(async (p, ct) => await GenerateLowStockReport())
        });
        stockCategory.Reports.Add(new ReportItem 
        { 
            Title = "حركة صنف تفصيلية", 
            Description = "تتبع صنف معين خلال فترة.",
            OpenCommand = new RelayCommand(_ => _navigationService.NavigateTo<ProductListViewModel>())
        });
        Categories.Add(stockCategory);

        // 2. تقارير الشركاء
        var partnerCategory = new ReportCategory 
        { 
            Title = "تقارير الشركاء", 
            IconKey = "IconUsers",
            Description = "كشوف حسابات العملاء والموردين وأرصدة الديون."
        };
        partnerCategory.Reports.Add(new ReportItem { Title = "أرصدة الشركاء (إجمالي)", Description = "قائمة بكل الشركاء وأرصدتهم الحالية.", 
            OpenCommand = new AsyncRelayCommand(async (p, ct) => await GeneratePartnerSummaryReport()) });
        partnerCategory.Reports.Add(new ReportItem { Title = "كشف حساب شريك", Description = "عرض حركات شريك محدد خلال فترة.",
            OpenCommand = new RelayCommand(_ => _navigationService.NavigateTo<PartnerAccountsViewModel>()) }); // Shortcut to accounts where statement is triggered
        partnerCategory.Reports.Add(new ReportItem { Title = "أعمار الديون", Description = "تحليل المبالغ المتأخرة حسب المدة.",
            OpenCommand = new AsyncRelayCommand(async (p, ct) => await GenerateDebtAgingReport()) });
        Categories.Add(partnerCategory);

        // 3. التقارير المالية
        var financeCategory = new ReportCategory 
        { 
            Title = "التقارير المالية", 
            IconKey = "IconSuccess",
            Description = "متابعة حركة الخزينة، الضرائب، والأرباح والخسائر."
        };
        financeCategory.Reports.Add(new ReportItem { Title = "يومية الخزينة", Description = "ملخص المقبوضات والمدفوعات اليومية." });
        financeCategory.Reports.Add(new ReportItem { Title = "تقرير الضرائب (VAT)", Description = "ملخص مبيعات ومشتريات وضرائب الفترة." });
        financeCategory.Reports.Add(new ReportItem { Title = "الأرباح والخسائر التقديرية", Description = "حساب صافي الربح المتوقع." });
        Categories.Add(financeCategory);
    }

    private async Task GenerateLowStockReport()
    {
        try
        {
            var products = await _mediator.Send(new GetLowStockProductsQuery());
            var settings = await _mediator.Send(new GetGeneralSettingsQuery());
            
            var pdf = _stockReportService.GenerateLowStockPdf(products, settings);
            
            var previewViewModel = App.ServiceProvider.GetRequiredService<ReportPreviewViewModel>();
            previewViewModel.LoadReport(pdf, "تقرير نواقص الأصناف");
            
            var previewWindow = new ReportPreviewWindow(previewViewModel);
            previewWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء إنشاء تقرير النواقص:\n{ex.Message}", "خطأ", 
                MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, 
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
    }

    private async Task GenerateInventoryValueReport()
    {
        try
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            var settings = await _mediator.Send(new GetGeneralSettingsQuery());
            
            var pdf = _stockReportService.GenerateInventoryValuePdf(products, settings);
            
            var previewViewModel = App.ServiceProvider.GetRequiredService<ReportPreviewViewModel>();
            previewViewModel.LoadReport(pdf, "تقرير جرد المخزون");
            
            var previewWindow = new ReportPreviewWindow(previewViewModel);
            previewWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء إنشاء تقرير الجرد:\n{ex.Message}", "خطأ", 
                MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, 
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
    }

    private async Task GeneratePartnerSummaryReport()
    {
        try
        {
            // Get all accounts (no search, no filter, large page size to get all)
            var response = await _mediator.Send(new GetPartnerAccountsQuery(null, null, null, 1, 1000));
            var settings = await _mediator.Send(new GetGeneralSettingsQuery());
            
            var pdf = _partnerReportService.GeneratePartnerSummaryPdf(response.Items, settings);
            
            var previewViewModel = App.ServiceProvider.GetRequiredService<ReportPreviewViewModel>();
            previewViewModel.LoadReport(pdf, "تقرير أرصدة الشركاء");
            
            var previewWindow = new ReportPreviewWindow(previewViewModel);
            previewWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء إنشاء تقرير الأرصدة:\n{ex.Message}", "خطأ", 
                MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, 
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
    }

    private async Task GenerateDebtAgingReport()
    {
        try
        {
            var agingData = await _mediator.Send(new GetDebtAgingQuery());
            var settings = await _mediator.Send(new GetGeneralSettingsQuery());
            
            var pdf = _partnerReportService.GenerateDebtAgingPdf(agingData, settings);
            
            var previewViewModel = App.ServiceProvider.GetRequiredService<ReportPreviewViewModel>();
            previewViewModel.LoadReport(pdf, "تقرير أعمار الديون");
            
            var previewWindow = new ReportPreviewWindow(previewViewModel);
            previewWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء إنشاء تقرير أعمار الديون:\n{ex.Message}", "خطأ", 
                MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, 
                MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }
    }
}

public class ReportCategory
{
    public string Title { get; set; } = string.Empty;
    public string IconKey { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ObservableCollection<ReportItem> Reports { get; } = new();
}

public class ReportItem
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICommand? OpenCommand { get; set; }
}
