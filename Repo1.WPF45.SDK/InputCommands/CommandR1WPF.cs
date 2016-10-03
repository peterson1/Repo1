using System;
using System.Windows;
using System.Windows.Input;
using PropertyChanged;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.InputCommands;

namespace Repo1.WPF45.SDK.InputCommands
{
    [ImplementPropertyChanged]
    public class CommandR1WPF : ICommandR1
    {
        protected Action   <object> _action;
        protected Predicate<object> _canExecute;


        public CommandR1WPF(Action<object> action, Predicate<object> canExecute = null, string buttonLabel = null)
        {
            _action      = action;
            _canExecute  = canExecute;
            CurrentLabel = buttonLabel;
        }


        public bool      OverrideEnabled   { get; set; } = true;
        public string    CurrentLabel      { get; set; }
        public bool      IsCheckable       { get; set; }
        public bool      IsChecked         { get; set; }
        public bool      IsBusy            { get; private set; }
        public bool      LastExecutedOK    { get; private set; }
        public DateTime  LastExecuteStart  { get; private set; }
        public DateTime  LastExecuteEnd    { get; private set; }



        public void Execute(object parameter)
        {
            if (IsBusy) return;
            if (!OverrideEnabled) return;

            IsBusy             = true;
            //var origOverride = OverrideEnabled;
            OverrideEnabled    = false;
            LastExecutedOK     = false;
            LastExecuteStart   = DateTime.Now;

            try
            {
                _action?.Invoke(parameter);
                LastExecutedOK = true;
            }
            catch (Exception ex)
            {
                //later: report error to server
                OnError(ex);
            }
            finally
            {
                LastExecuteEnd    = DateTime.Now;
                IsBusy            = false;
                //OverrideEnabled = origOverride;
            }
        }


        private void OnError(Exception error)
        {
            var targ  = _action.Target.GetType().Name;
            var methd = _action.Method.Name;
            var cap   = $"Error on ‹{targ}› {methd}";
            var msg   = error.Info();

            MessageBox.Show(msg, cap,
                MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public bool CanExecute(object parameter)
        {
            if (IsBusy) return false;
            if (!OverrideEnabled) return false;
            return _canExecute?.Invoke(parameter) ?? true;
        }


        public void ExecuteIfItCan(object param = null)
        {
            if (CanExecute(param)) Execute(param);
        }
    }
}
