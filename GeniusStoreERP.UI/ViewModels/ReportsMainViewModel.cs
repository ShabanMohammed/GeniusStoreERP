using GeniusStoreERP.UI.Common;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GeniusStoreERP.UI.ViewModels.Partners;
using GeniusStoreERP.UI.Services;
using GeniusStoreERP.Application.Stock.Products.Queries.GetLowStockProducts;
using GeniusStoreERP.Application.Stock.Products.Queries.GetAllProducts;
using GeniusStoreERP.Application.GeneralSettings.Queries.GetGeneralSettings;
using GeniusStoreERP.Application.Common.Interfaces;
using MediatR;
using System.Threading.Tasks;
using System;
using GeniusStoreERP.UI.Views;

namespace GeniusStoreERP.UI.ViewModels;

public class ReportsMainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IMediator _mediator;
    private readonly IStockReportService _stockReportService;

    public ObservableCollection<ReportCategory> Categories { get; } = new();

    public ReportsMainViewModel()
        : this(
            App.ServiceProvider.GetRequiredService<INavigationService>(),
            App.ServiceProvider.GetRequiredService<IMediator>(),
            App.ServiceProvider.GetRequiredService<IStockReportService>())
    {
    }

    public ReportsMainViewModel(
        INavigationService navigationService, 
        IMediator mediator,
        IStockReportService stockReportService)
    {
        _navigationService = navigationService;
        _mediator = mediator;
        _stockReportService = stockReportService;
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
        stockCategory.Reports.Add(new ReportItem { Title = "حركة صنف تفصيلية", Description = "تتبع صنف معين خلال فترة." });
        Categories.Add(stockCategory);

        // 2. تقارير الشركاء
        var partnerCategory = new ReportCategory 
        { 
            Title = "تقارير الشركاء", 
            IconKey = "IconUsers",
            Description = "كشوف حسابات العملاء والموردين وأرصدة الديون."
        };
        partnerCategory.Reports.Add(new ReportItem { Title = "أرصدة الشركاء (إجمالي)", Description = "قائمة بكل الشركاء وأرصدتهم الحالية.", 
            OpenCommand = new RelayCommand(_ => _navigationService.NavigateTo<PartnerAccountsViewModel>()) });
        partnerCategory.Reports.Add(new ReportItem { Title = "كشف حساب شريك", Description = "عرض حركات شريك محدد خلال فترة.",
            OpenCommand = new RelayCommand(_ => _navigationService.NavigateTo<PartnerAccountsViewModel>()) }); // Shortcut to accounts where statement is triggered
        partnerCategory.Reports.Add(new ReportItem { Title = "أعمار الديون", Description = "تحليل المبالغ المتأخرة حسب المدة." });
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
        var products = await _mediator.Send(new GetLowStockProductsQuery());
        var settings = await _mediator.Send(new GetGeneralSettingsQuery());
        
        var pdf = _stockReportService.GenerateLowStockPdf(products, settings);
        
        var previewViewModel = App.ServiceProvider.GetRequiredService<ReportPreviewViewModel>();
        previewViewModel.LoadReport(pdf, "تقرير نواقص الأصناف");
        
        var previewWindow = new ReportPreviewWindow(previewViewModel);
        previewWindow.ShowDialog();
    }

    private async Task GenerateInventoryValueReport()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        var settings = await _mediator.Send(new GetGeneralSettingsQuery());
        
        var pdf = _stockReportService.GenerateInventoryValuePdf(products, settings);
        
        var previewViewModel = App.ServiceProvider.GetRequiredService<ReportPreviewViewModel>();
        previewViewModel.LoadReport(pdf, "تقرير جرد المخزون");
        
        var previewWindow = new ReportPreviewWindow(previewViewModel);
        previewWindow.ShowDialog();
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
