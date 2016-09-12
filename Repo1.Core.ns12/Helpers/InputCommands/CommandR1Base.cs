using System;

namespace Repo1.Core.ns12.Helpers.InputCommands
{
    public abstract class CommandR1Base : ICommandR1
    {
        public CommandR1Base(Action<object> action, Predicate<object> canExecute = null)
        {
            _action     = action;
            _canExecute = canExecute;
        }


        private EventHandler _canExecuteChanged;
        public virtual event EventHandler  CanExecuteChanged
        {
            add    { _canExecuteChanged -= value; _canExecuteChanged += value; }
            remove { _canExecuteChanged -= value; }
        }

        protected Action<object>    _action;
        protected Predicate<object> _canExecute;

        protected abstract void OnError(Exception ex);

        public void Execute(object parameter)
        {
            if (IsBusy) return;
            if (!OverrideEnabled) return;
            IsBusy = true;
            LastExecutedOK = false;
            LastExecuteStart = DateTime.Now;

            try
            {
                _action?.Invoke(parameter);
                LastExecutedOK = true;
            }
            catch (Exception ex)
            {
                //todo: report error to server
                OnError(ex);
            }
            finally
            {
                LastExecuteEnd = DateTime.Now;
                IsBusy = false;
            }
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


        public string    CurrentLabel      { get; set; }
        public bool      IsBusy            { get; set; }
        public bool      IsCheckable       { get; set; }
        public bool      IsChecked         { get; set; }
        public bool      OverrideEnabled   { get; set; } = true;
        public bool      LastExecutedOK    { get; private set; }
        public DateTime  LastExecuteStart  { get; private set; }
        public DateTime  LastExecuteEnd    { get; private set; }
    }
}
