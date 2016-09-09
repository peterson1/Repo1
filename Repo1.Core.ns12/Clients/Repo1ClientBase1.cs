using System;
using System.Threading.Tasks;
using Repo1.Core.ns12.Helpers;

namespace Repo1.Core.ns12.Clients
{
    public abstract class Repo1ClientBase1 : IRepo1Client
    {
        private bool _keepChecking;

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
                _statusChanged.Raise("1");

                await Task.Delay(1000 * 2);
                _statusChanged.Raise("2");

                await Task.Delay(1000 * 2);
                _statusChanged.Raise("3");
            }
        }
    }
}
