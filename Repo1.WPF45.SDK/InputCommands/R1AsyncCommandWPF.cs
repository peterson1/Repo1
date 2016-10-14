using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PropertyChanged;
using Repo1.Core.ns11.Extensions.ExceptionExtensions;
using Repo1.Core.ns11.InputCommands;

namespace Repo1.WPF45.SDK.InputCommands
{
    [ImplementPropertyChanged]
    public class R1AsyncCommandWPF : IR1Command
    {
        //protected Action            _action;
        protected Func<Task>        _task;
        protected Predicate<object> _canExecute;




        //public static CommandR1WPF New(Action action, Predicate<object> canExecute = null, string buttonLabel = null)
        //    => new CommandR1WPF(action, canExecute, buttonLabel);


        internal R1AsyncCommandWPF(Func<Task> task, Predicate<object> canExecute, string buttonLabel)
        {
            _task        = task;
            _canExecute  = canExecute;
            CurrentLabel = buttonLabel;
        }

        //private CommandR1WPF(Action action, Predicate<object> canExecute, string buttonLabel)
        //{
        //    _action      = action;
        //    _canExecute  = canExecute;
        //    CurrentLabel = buttonLabel;
        //}


        public string    CurrentLabel      { get; set; }
        public bool      IsBusy            { get; protected set; }
        public bool      IsCheckable       { get; set; }
        public bool      IsChecked         { get; set; }
        public bool      OverrideEnabled   { get; set; } = true;
        public bool      DisableWhenDone   { get; set; }
        public bool      LastExecutedOK    { get; protected set; }
        public DateTime  LastExecuteStart  { get; protected set; }
        public DateTime  LastExecuteEnd    { get; protected set; }



        public async void Execute(object parameter)
        {
            if (IsBusy) return;
            if (!OverrideEnabled) return;

            IsBusy             = true;
            var origOverride   = OverrideEnabled;
            OverrideEnabled    = false;
            LastExecutedOK     = false;
            LastExecuteStart   = DateTime.Now;

            try
            {
                //_action?.Invoke(parameter);
                await _task.Invoke();
                LastExecutedOK = true;
            }
            catch (Exception ex)
            {
                //later: report error to server
                OnError(ex);
            }
            finally
            {
                LastExecuteEnd  = DateTime.Now;
                IsBusy          = false;
                OverrideEnabled = DisableWhenDone ? false : origOverride;
                CommandManager.InvalidateRequerySuggested();
            }
        }


        protected virtual void OnError(Exception error)
        {
            //var targ  = _action.Target.GetType().Name;
            //var methd = _action.Method.Name;
            //var cap   = $"Error on ‹{targ}› {methd}";
            var cap = "Error on Task";
            var msg   = error.Info(true, true);

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
