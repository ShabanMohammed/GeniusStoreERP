using GeniusStoreERP.UI.Common;
using Microsoft.Extensions.DependencyInjection;

namespace GeniusStoreERP.UI.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public BaseViewModel? CurrentViewModel { get; private set; }

        // جديد: حدث يُطلق عند التنقل ليستطيع MainWindowViewModel (أو أي مستمع) التحديث
        public event Action<BaseViewModel>? Navigated;

        public TViewModel NavigateTo<TViewModel>() where TViewModel : BaseViewModel
        {
            var vm = _serviceProvider.GetRequiredService<TViewModel>();
            vm.Initialize(null);
            CurrentViewModel = vm;
            Navigated?.Invoke(vm);
            return vm;
        }

        public TViewModel NavigateTo<TViewModel>(object? parameter) where TViewModel : BaseViewModel
        {
            var vm = _serviceProvider.GetRequiredService<TViewModel>();
            vm.Initialize(parameter);
            CurrentViewModel = vm;
            Navigated?.Invoke(vm);
            return vm;
        }

        public TViewModel ShowWindow<TViewModel>(object? parameter, string title = "") where TViewModel : BaseViewModel
        {
            var vm = _serviceProvider.GetRequiredService<TViewModel>();
            vm.Initialize(parameter);

            //// هذا الجزء يتطلب معرفة النافذة المرتبطة بالـ ViewModel
            //// كحل سريع وموجه للمهمة، سنقوم بفتح ReportPreviewWindow مباشرة إذا كان الـ ViewModel هو ReportPreviewViewModel
            //if (vm is ReportPreviewViewModel reportVm)
            //{
            //    var window = new GSM.UI.Views.ReportPreviewWindow(reportVm);
            //    if (!string.IsNullOrEmpty(title)) window.Title = title;
            //    window.Show();
            //}

            return vm;
        }
    }
}
