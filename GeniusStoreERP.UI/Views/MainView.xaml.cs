using System.Windows;

namespace GeniusStoreERP.UI.Views
{
    public partial class MainView : Window
    {
        public MainView(ViewModels.MainViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
