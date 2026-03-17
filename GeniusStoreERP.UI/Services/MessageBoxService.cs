using GeniusStoreERP.UI.ViewModels;
using GeniusStoreERP.UI.Views;
using System.Windows;

namespace GeniusStoreERP.UI.Services;

public static class MessageBoxService
{
    public static void ShowSuccess(string message, string title = "نجاح")
    {
        Show(message, title, MessageBoxType.Success);
    }

    public static void ShowError(string message, string title = "خطأ")
    {
        Show(message, title, MessageBoxType.Error);
    }

    public static void ShowWarning(string message, string title = "تحذير")
    {
        Show(message, title, MessageBoxType.Warning);
    }

    public static void ShowInfo(string message, string title = "معلومات")
    {
        Show(message, title, MessageBoxType.Info);
    }

    public static MessageBoxResult ShowConfirmation(string message, string title = "تأكيد")
    {
        return Show(message, title, MessageBoxType.Confirmation);
    }

    private static MessageBoxResult Show(string message, string title, MessageBoxType type)
    {
        var window = new MessageBoxView();
        var viewModel = new MessageBoxViewModel(window)
        {
            Message = message,
            Title = title,
            Type = type
        };

        window.DataContext = viewModel;
        window.ShowDialog();

        return viewModel.Result;
    }
}
