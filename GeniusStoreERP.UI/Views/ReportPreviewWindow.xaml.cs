using GeniusStoreERP.UI.ViewModels;
using Microsoft.Web.WebView2.Core;
using System.Windows;

namespace GeniusStoreERP.UI.Views
{
    public partial class ReportPreviewWindow : Window
    {
        private ReportPreviewViewModel? ViewModel => DataContext as ReportPreviewViewModel;

        public ReportPreviewWindow(ReportPreviewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            Loaded += ReportPreviewWindow_Loaded;
        }

        private async void ReportPreviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // تهيئة WebView2
                await PdfViewer.EnsureCoreWebView2Async();
                
                // تحميل PDF إذا كان موجوداً
                if (ViewModel?.PdfData != null)
                {
                    await LoadPdfAsync(ViewModel.PdfData);
                }

                // الاستماع لتغييرات ZoomLevel
                if (ViewModel != null)
                {
                    ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تهيئة عارض PDF:\n{ex.Message}", 
                    "خطأ", MessageBoxButton.OK, MessageBoxImage.Error,
                    MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        private async void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReportPreviewViewModel.ZoomLevel))
            {
                await ApplyZoomAsync();
            }
        }

        private async Task LoadPdfAsync(byte[] pdfData)
        {
            try
            {
                // تحويل PDF إلى Base64
                var base64 = Convert.ToBase64String(pdfData);
                var dataUrl = $"data:application/pdf;base64,{base64}";

                // إنشاء HTML لعرض PDF
                var html = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='utf-8'>
                        <style>
                            body, html {{
                                margin: 0;
                                padding: 0;
                                width: 100%;
                                height: 100%;
                                overflow: hidden;
                            }}
                            iframe {{
                                border: none;
                                width: 100%;
                                height: 100%;
                            }}
                        </style>
                    </head>
                    <body>
                        <iframe src='{dataUrl}' width='100%' height='100%'></iframe>
                    </body>
                    </html>";

                PdfViewer.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل PDF:\n{ex.Message}",
                    "خطأ", MessageBoxButton.OK, MessageBoxImage.Error,
                    MessageBoxResult.OK, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        private async Task ApplyZoomAsync()
        {
            if (ViewModel == null || PdfViewer.CoreWebView2 == null) return;

            try
            {
                var zoomFactor = ViewModel.ZoomLevel / 100.0;
                PdfViewer.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "appassets.example", 
                    Environment.CurrentDirectory, 
                    CoreWebView2HostResourceAccessKind.Allow);
                
                await PdfViewer.CoreWebView2.ExecuteScriptAsync($"document.body.style.zoom = '{zoomFactor}';");
            }
            catch
            {
                // Ignore zoom errors
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            
            PdfViewer?.Dispose();
            base.OnClosed(e);
        }
    }
}
