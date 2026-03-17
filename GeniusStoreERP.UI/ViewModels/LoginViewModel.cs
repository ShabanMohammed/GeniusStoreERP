using GeniusStoreERP.Application.Users.Commands.Login;
using GeniusStoreERP.UI.Common;
using MediatR;
using FluentValidation;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using GeniusStoreERP.UI.Services;

namespace GeniusStoreERP.UI.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string userName = "";

    public string UserName
    {
        get => userName;
        set => SetProperty(ref userName, value);
    }
    private string _password = "";

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }
    private readonly IMediator _mediator;

    public LoginViewModel(IMediator mediator)
    {
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        _mediator = mediator;
    }

    private async Task LoginAsync(object? arg1, CancellationToken token)
    {
        try
        {
            var command = new LoginCommand(UserName, Password);
            var result = await _mediator.Send(command, token);
            
            // نجاح تسجيل الدخول
            var mainView = App.ServiceProvider.GetRequiredService<Views.MainView>();
            if (mainView.DataContext is MainViewModel mainVm)
            {
                mainVm.FullName = result.FullName;
                mainVm.UserRole = result.Role;
            }
            
            mainView.Show();
            
            // إغلاق شاشة تسجيل الدخول
            System.Windows.Application.Current.MainWindow.Close();
            System.Windows.Application.Current.MainWindow = mainView;
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBoxService.ShowError(ex.Message, "فشل تسجيل الدخول");
        }
        catch (ValidationException ex)
        {
            var errors = string.Join(Environment.NewLine, ex.Errors.Select(e => e.ErrorMessage));
            MessageBoxService.ShowWarning(errors, "تنبيه");
        }
        catch (Exception )
        {
            MessageBoxService.ShowError("حدث خطأ غير متوقع أثناء محاولة تسجيل الدخول. يرجى المحاولة مرة أخرى.", "خطأ");
            
        }


    }
}
