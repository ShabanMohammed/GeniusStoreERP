using System.Windows.Input;

namespace GeniusStoreERP.UI.Common;

public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object?>? _caneExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<object> execute, Predicate<object?>? caneExecute = null)
    {
        _execute = execute;
        _caneExecute = caneExecute;
    }

    public bool CanExecute(object? parameter) =>
        _caneExecute?.Invoke(parameter) ?? true;


    public void Execute(object? parameter) =>
        _execute(parameter!);

}
