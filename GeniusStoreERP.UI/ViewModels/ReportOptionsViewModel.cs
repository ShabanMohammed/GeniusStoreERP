using GeniusStoreERP.UI.Common;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels
{
    public class ReportOptionsViewModel : BaseViewModel
    {
        private DateTime _fromDate = DateTime.Today.AddDays(-30);
        public DateTime FromDate
        {
            get => _fromDate;
            set => SetProperty(ref _fromDate, value);
        }

        private DateTime _toDate = DateTime.Today;
        public DateTime ToDate
        {
            get => _toDate;
            set => SetProperty(ref _toDate, value);
        }

        // 0 = All, 1 = Sales, 2 = Purchases
        private int _selectedTypeIndex = 0;
        public int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set => SetProperty(ref _selectedTypeIndex, value);
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<bool>? OnRequestClose;

        public ReportOptionsViewModel()
        {
            ConfirmCommand = new RelayCommand(_ => OnRequestClose?.Invoke(true));
            CancelCommand = new RelayCommand(_ => OnRequestClose?.Invoke(false));
        }
    }
}
