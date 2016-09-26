using System;
using System.Windows.Input;

namespace Repo1.WPF452.SDK.InputCommands
{
    public class RelayCommand : ICommand
    {
        private Action<object>     _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute    = execute;
            _canExecute = canExecute;
        }

        public void Execute   (object parameter) => _execute(parameter);
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
    }
}
