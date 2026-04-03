using GeniusStoreERP.UI.Common;
using GeniusStoreERP.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels
{
    public class ReportPreviewViewModel : BaseViewModel
    {
        private readonly ILogger<ReportPreviewViewModel> _logger;
        
        private byte[]? _pdfData;
        private string _reportTitle = string.Empty;
        private string _statusText = "جاهز";
        private int _zoomLevel = 100;
        private string _pageInfo = string.Empty;
        
        // ديليجيت (Delegate) لحمل منطق التصدير الخاص بكل تقرير
        private Action<string>? _exportAction;

        public byte[]? PdfData
        {
            get => _pdfData;
            set => SetProperty(ref _pdfData, value);
        }

        public string ReportTitle
        {
            get => _reportTitle;
            set => SetProperty(ref _reportTitle, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public int ZoomLevel
        {
            get => _zoomLevel;
            set => SetProperty(ref _zoomLevel, value);
        }

        public string PageInfo
        {
            get => _pageInfo;
            set => SetProperty(ref _pageInfo, value);
        }

        public ICommand ExportToExcelCommand { get; }
        public ICommand SavePdfCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }

        public ReportPreviewViewModel(ILogger<ReportPreviewViewModel> logger)
        {
            _logger = logger;

            // التصدير مفعل فقط إذا تم تمرير دالة تصدير
            ExportToExcelCommand = new RelayCommand(_ => ExportToExcel(), _ => _exportAction != null);
            
            SavePdfCommand = new RelayCommand(_ => SavePdf(), _ => PdfData != null);
            PrintCommand = new RelayCommand(_ => Print(), _ => PdfData != null);
            CloseCommand = new RelayCommand(Close);
            ZoomInCommand = new RelayCommand(_ => ZoomIn());
            ZoomOutCommand = new RelayCommand(_ => ZoomOut());
        }

        public override void Initialize(object? parameter)
        {
            if (parameter is byte[] pdfData)
            {
                LoadReport(pdfData, "معاينة التقرير");
            }
        }

        /// <summary>
        /// تحميل التقرير والإعدادات للعرض
        /// </summary>
        /// <param name="pdfData">بيانات ملف PDF</param>
        /// <param name="title">عنوان التقرير</param>
        /// <param name="info">معلومات إضافية (مثل عدد السجلات)</param>
        /// <param name="exportAction">دالة التصدير إلى Excel (اختياري)</param>
        public void LoadReport(byte[] pdfData, string title, string info = "", Action<string>? exportAction = null)
        {
            PdfData = pdfData;
            ReportTitle = title;
            PageInfo = info;
            _exportAction = exportAction;
            
            StatusText = "تم تحميل التقرير بنجاح";
            
            _logger.LogInformation("تم تحميل تقرير: {Title} - {Info}", title, info);
        }

        private void ExportToExcel()
        {
            if (_exportAction == null) return;

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    FileName = $"{ReportTitle.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    DefaultExt = ".xlsx"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    StatusText = "جاري التصدير...";
                    
                    // تنفيذ دالة التصدير الممررة
                    _exportAction(saveDialog.FileName);
                    
                    StatusText = "تم التصدير بنجاح";
                    
                    MessageBoxService.ShowSuccess($"تم التصدير إلى:\n{saveDialog.FileName}");
                    _logger.LogInformation("تم تصدير التقرير إلى Excel: {FilePath}", saveDialog.FileName);

                    // فتح الملف
                    var result = MessageBox.Show("هل تريد فتح الملف الآن؟", "تصدير Excel", 
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText = "فشل التصدير";
                MessageBoxService.ShowError($"حدث خطأ أثناء التصدير:\n{ex.Message}");
                _logger.LogError(ex, "فشل تصدير التقرير إلى Excel");
            }
        }

        private void SavePdf()
        {
            if (PdfData == null)
            {
                MessageBoxService.ShowWarning("لا يوجد تقرير للحفظ");
                return;
            }

            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"قائمة_الأسعار_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    DefaultExt = ".pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    StatusText = "جاري الحفظ...";
                    File.WriteAllBytes(saveDialog.FileName, PdfData);
                    StatusText = "تم الحفظ بنجاح";
                    
                    MessageBoxService.ShowSuccess($"تم حفظ الملف إلى:\n{saveDialog.FileName}");
                    _logger.LogInformation("تم حفظ التقرير PDF: {FilePath}", saveDialog.FileName);

                    // فتح الملف
                    var result = MessageBox.Show("هل تريد فتح الملف الآن؟", "حفظ PDF",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText = "فشل الحفظ";
                MessageBoxService.ShowError($"حدث خطأ أثناء الحفظ:\n{ex.Message}");
                _logger.LogError(ex, "فشل حفظ التقرير PDF");
            }
        }

        private void Print()
        {
            if (PdfData == null)
            {
                MessageBoxService.ShowWarning("لا يوجد تقرير للطباعة");
                return;
            }

            try
            {
                // حفظ مؤقت للطباعة
                var tempFile = Path.Combine(Path.GetTempPath(), $"print_temp_{Guid.NewGuid()}.pdf");
                File.WriteAllBytes(tempFile, PdfData);

                // فتح للطباعة
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempFile,
                    UseShellExecute = true,
                    Verb = "print"
                });

                StatusText = "تم إرسال التقرير للطباعة";
                _logger.LogInformation("تم إرسال التقرير للطباعة");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError($"حدث خطأ أثناء الطباعة:\n{ex.Message}");
                _logger.LogError(ex, "فشل طباعة التقرير");
            }
        }

        private void Close(object? parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private void ZoomIn()
        {
            if (ZoomLevel < 200)
            {
                ZoomLevel += 10;
                StatusText = $"التكبير: {ZoomLevel}%";
            }
        }

        private void ZoomOut()
        {
            if (ZoomLevel > 50)
            {
                ZoomLevel -= 10;
                StatusText = $"التصغير: {ZoomLevel}%";
            }
        }
    }
}
