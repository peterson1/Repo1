using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Repo1.Core.ns12.Helpers;
using Repo1.Core.ns12.Helpers.PropertyChangedExtensions;

namespace Repo1.Core.ns12.Clients
{
    public abstract class Repo1ClientBase1 : IRepo1Client
    {
        public Repo1ClientBase1(string userName, string password, string apiBaseURL)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private bool _keepChecking;


        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                PropertyChanged.Raise(nameof(Status));
                _statusChanged.Raise(_status);
            }
        }


        private      EventHandler<EArg<string>> _statusChanged;
        public event EventHandler<EArg<string>>  StatusChanged
        {
            add    { _statusChanged -= value; _statusChanged += value; }
            remove { _statusChanged -= value; }
        }


        protected abstract void RunOnNewThread(Task task);


        public void StartUpdateCheckLoop()
        {
            _keepChecking = true;
            RunOnNewThread(ExecuteUpdateCheckLoop());
        }


        public void StopUpdateCheckLoop()
        {
            _keepChecking = false;
        }


        private async Task ExecuteUpdateCheckLoop()
        {
            while (_keepChecking)
            {
                await Task.Delay(1000 * 2);
                Status = "1";

                await Task.Delay(1000 * 2);
                Status = "2";

                await Task.Delay(1000 * 2);
                Status = "3";
            }
        }

        public virtual void RaisePropertyChanged(string propertyName)
            => PropertyChanged.Raise(propertyName);
    }
}
