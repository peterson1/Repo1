using System;
using System.Windows;
using System.Windows.Input;
using PropertyChanged;
using Repo1.Core.ns12.Helpers.InputCommands;
using Repo1.Core.ns12.Helpers.ExceptionExtensions;

namespace Repo1.WPF452.SDK.Helpers.InputCommands
{
    [ImplementPropertyChanged]
    public class CommandR1WPF : CommandR1Base
    {
        public CommandR1WPF(Action<object> action, Predicate<object> canExecute = null) : base(action, canExecute)
        {
        }


        public override event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        protected override void OnError(Exception error)
        {
            var targ  = _action.Target.GetType().Name;
            var methd = _action.Method.Name;
            var cap   = $"Error on ‹{targ}› {methd}";
            var msg   = error.Info();

            MessageBox.Show(msg, cap,
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
