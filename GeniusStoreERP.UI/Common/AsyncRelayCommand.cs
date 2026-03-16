using System.Windows.Input;

namespace GeniusStoreERP.UI.Common
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object?, CancellationToken, Task> _executeAsync;
        private readonly Predicate<object?>? _canExecute;
        private CancellationTokenSource _cts = new();
        private bool _isExecuting;

        public AsyncRelayCommand(Func<object?, CancellationToken, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }


        public bool CanExecute(object? parameter) =>
            !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);


        public async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        private async Task ExecuteAsync(object? parameter)
        {
            if (!CanExecute(parameter)) return;
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                _cts = new CancellationTokenSource();

                await _executeAsync(parameter, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // تم إلغاء العملية من قبل المستخدم، لا داعي لإظهار خطأ
            }
            catch (Exception)
            {
                // هنا يمكن التعامل مع الأخطاء غير المتوقعة
                throw;
            }

            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }
        public void Cancel() => _cts.Cancel();
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

    }
}
