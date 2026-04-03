using GeniusStoreERP.UI.ViewModels;
using System.Windows;

namespace GeniusStoreERP.UI.Views
{
    public partial class ReportOptionsWindow : Window
    {
        public ReportOptionsWindow(ReportOptionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.OnRequestClose += (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
