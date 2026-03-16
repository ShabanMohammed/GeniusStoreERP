using GeniusStoreERP.UI.Common;
using System.Windows;
using System.Windows.Input;

namespace GeniusStoreERP.UI.ViewModels;

public enum MessageBoxType
{
    Success,
    Error,
    Warning,
    Info,
    Confirmation
}

public class MessageBoxViewModel : BaseViewModel
{
    private string _title = "";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _message = "";
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private MessageBoxType _type;
    public MessageBoxType Type
    {
        get => _type;
        set
        {
            if (SetProperty(ref _type, value))
            {
                OnPropertyChanged(nameof(IconKey));
                OnPropertyChanged(nameof(StatusBrushKey));
                OnPropertyChanged(nameof(IsConfirmation));
            }
        }
    }

    public string IconKey => Type switch
    {
        MessageBoxType.Success => "IconSuccess",
        MessageBoxType.Error => "IconError",
        MessageBoxType.Warning => "IconWarning",
        MessageBoxType.Info => "IconInformation",
        MessageBoxType.Confirmation => "IconInformation",
        _ => "IconInformation"
    };

    public string StatusBrushKey => Type switch
    {
        MessageBoxType.Success => "StatusSuccessBrush",
        MessageBoxType.Error => "StatusErrorBrush",
        MessageBoxType.Warning => "StatusWarningBrush",
        MessageBoxType.Info => "StatusInfoBrush",
        MessageBoxType.Confirmation => "StatusInfoBrush",
        _ => "StatusInfoBrush"
    };

    public bool IsConfirmation => Type == MessageBoxType.Confirmation;

    public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand YesCommand { get; }
    public ICommand NoCommand { get; }

    private readonly Window _window;

    public MessageBoxViewModel(Window window)
    {
        _window = window;
        OkCommand = new RelayCommand(_ => CloseWithResult(MessageBoxResult.OK));
        CancelCommand = new RelayCommand(_ => CloseWithResult(MessageBoxResult.Cancel));
        YesCommand = new RelayCommand(_ => CloseWithResult(MessageBoxResult.Yes));
        NoCommand = new RelayCommand(_ => CloseWithResult(MessageBoxResult.No));
    }

    private void CloseWithResult(MessageBoxResult result)
    {
        Result = result;
        _window.DialogResult = true;
        _window.Close();
    }
}
